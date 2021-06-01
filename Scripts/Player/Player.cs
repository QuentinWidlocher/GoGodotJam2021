using Godot;
using System;
using Helpers;
using static Helpers.TaskHelpers;

public class Player : KinematicBody2D
{
    [Export] public float Gravity = 40f;
    [Export] public float Acceleration = 150f;
    [Export] public float MaxVelocity = 500f;
    [Export] public float Friction = 0.8f;
    [Export] public float JumpVelocity = 500f;
    [Export] public float JumpTime = 0.15f;
    [Export] public float DashForce = 1500f;
    [Export] public int DashDuration = 150;
    [Export] public int DashCoolDown = 100;
    [Export] public float KnockbackForce = 200;
    [Export] public int InvicibilityCoolDown = 500;
    [Export] public int WallJumpKnockback = 1500;

    [Signal]
    public delegate void HealthChange(float newValue);

    // MaxJumps is public to change the number of max jumps from other classes (is there a better way to do this?)
    public int MaxJumps => _statSystem.PlayerStat.HasDoubleJump ? 2 : 1;
    public int RemainingHeal;

    private float _healthPoints;

    public float HealthPoints
    {
        get => _healthPoints;
        set
        {
            _healthPoints = value;
            EmitSignal(nameof(HealthChange), value);
        }
    }
    
    private StatSystem _statSystem = null!;
    private SceneSwitcher _sceneSwitcher = null!;
    private RayCast2D _rayCast = null!;

    private Vector2 _vel = Vector2.Zero;
    private int _jumps = 0;
    private float _jumpTimer = 0;
    private readonly PackedScene _bolt = GD.Load<PackedScene>("res://Scenes/Player/Bolt.tscn");
    private bool _isFacingRight = true;
    private bool _isAimingUp = true;
    private bool _isJumpFromWall = true;
    private bool _hasLanded = false;
    private Vector2 _knockingBack; // over my shouuulder 🎵
    private AnimatedSprite _sprite = null!;
    private AnimationPlayer _animations = null!;

    private bool _isDashing;
    private bool _canDash = true;

    private bool OnGround => _rayCast.IsColliding();

    private Vector2 FacingDirection => _isFacingRight ? Vector2.Right : Vector2.Left;
    private bool HaveJumpsLeft => _jumps < MaxJumps;

    private bool _vulnerable = true;

    private bool Vulnerable
    {
        get => _vulnerable;
        set
        {
            _vulnerable = value;
            // Uncheck the player layer and the enemy mask so we can walk through them 
            SetCollisionLayerBit(1, value);
            SetCollisionMaskBit(2, value);

            byte alpha = (byte) (value ? 255 : 255 / 2);
            Modulate = Color.Color8(255, 255, 255, alpha);
        }
    }

    public override void _Ready()
    {
        _sprite = (AnimatedSprite) GetNode("AnimatedSprite");
        _animations = (AnimationPlayer) GetNode("AnimationPlayer");
        _statSystem = GetNode<StatSystem>("/root/StatSystem");
        _sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
        _rayCast = GetNode<RayCast2D>("RayCast2D");

        HealthPoints = _statSystem.PlayerStat.HealthPoints;
        RemainingHeal = _statSystem.PlayerStat.MaxHeals;
    }

    public override void _PhysicsProcess(float delta)
    {
        GetInput(delta);

        _vel = MoveAndSlide(_vel, Vector2.Up);

        for (int i = 0; i < GetSlideCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.Collider is Spikes && _vulnerable)
            {
                // Spikes deals 10% of the player max HP
                Hit(_statSystem.PlayerStat.HealthPoints * 0.1f, collision.Position);
                // Lazy to override the Hit but idc
                _knockingBack = new Vector2(_isFacingRight ? -1 : 1, -0.5f) * KnockbackForce;
            }
        }

        if (IsOnFloor() || OnGround)
        {
            if (!_hasLanded)
            {
                //_animations.Play("landing");
            }
            
            if (Input.IsActionPressed("left") || Input.IsActionPressed("right"))
                _sprite.Play("run");

            else
                _sprite.Play("idle");

            _hasLanded = true;

            _jumps = 0;
        }
        else
        {
            _hasLanded = false;
        }

        // Preventing the player from continuing their jump when bonking on a ceiling
        if (IsOnCeiling())
        {
            _jumpTimer = JumpTime;
        }

        _sprite.FlipH = !_isFacingRight;

        // Setting sprites by order of priority
        if (_vel.y > 0 && !OnGround)
            _sprite.Play("fall");
        if (_vel.y > 400 && !OnGround)
            _sprite.Play("fall_fast");
        if (IsOnWall() && !(IsOnFloor() || OnGround) && _statSystem.PlayerStat.HasWallJump)
            _sprite.Play("wall_slide");
        if (Input.IsActionPressed("attack_main"))
            _sprite.Play("attack");
        if (_isDashing)
            _sprite.Play("dash");
    }

    public void GetInput(float delta)
    {
        if (_knockingBack != Vector2.Zero)
        {
            _vel += _knockingBack;
            _knockingBack = _knockingBack.LinearInterpolate(Vector2.Zero, .7f);
            if (_knockingBack.Length() < 1)
            {
                _knockingBack = Vector2.Zero;
            }
            else
            {
                return;   
            }
        }

        if (!_isDashing)
        {
            if (Input.IsActionPressed("left"))
            {
                // Applying acceleration
                _vel.x -= Input.GetActionStrength("left") * Acceleration;
                // Clamping _vel.x only inside the input event because that way only limit the max velocity caused by input
                // This means that we can go past the maximum velocity if we apply acceleration elsewhere (e.g. dashes, cannons etc.)
                Math.Clamp(_vel.x, -MaxVelocity, MaxVelocity);
                _isFacingRight = false;
            }

            if (Input.IsActionPressed("right"))
            {
                // See above for explanations
                _vel.x += Input.GetActionStrength("right") * Acceleration;
                Math.Clamp(_vel.x, -MaxVelocity, MaxVelocity);
                _isFacingRight = true;
            }

            if (Input.IsActionJustPressed("attack_main"))
            {
                Attack();
            }

            if (Input.IsActionJustPressed("dash") && _statSystem.PlayerStat.HasDash)
            {
                Dash();
            }

            if (Input.IsActionJustPressed("heal"))
            {
                Heal();
            }
        }
        else
        {
            _vel = FacingDirection * DashForce;
        }

        // Simulating air resistance and friction (WARNING: Extremely difficult partial differential equation being calculated, proceed with caution)
        _vel.x *= Friction;

        // Applying the gravity before jumping so the jump velocity isn't affected by gravity
        if (!(IsOnFloor() || OnGround))
            _vel.y += Gravity;

        // When the jump button is held, _jumpTimer counts up to only let the player gain y velocity until JumpTime is reached
        if (Input.IsActionJustPressed("jump") && _statSystem.PlayerStat.HasWallJump)
        {
            if (IsOnWall() && !(IsOnFloor() || OnGround)) {
                _vel.x += -Mathf.Sign(_vel.x) * WallJumpKnockback;
                _isJumpFromWall = true;
            }
        }

        if (Input.IsActionPressed("jump"))
        {
            _jumpTimer += delta;
            if (_jumpTimer < JumpTime && (_jumps < MaxJumps || _isJumpFromWall))
            {
                _vel.y = -JumpVelocity;
                _sprite.Play("jump");
            }
        }

        // Resetting jump variables once button is released
        if (Input.IsActionJustReleased("jump"))
        {
            if (!_isJumpFromWall)
                _jumps++;
            _jumpTimer = 0;
            if (_jumps <= MaxJumps)
                _sprite.Play("fall");
            _isJumpFromWall = false;
        }

        // This is just so _jumpTimer is zeroed even if the player hits a ceiling (which would set the timer to JumpTime)
        if (!Input.IsActionPressed("jump") && _jumpTimer >= JumpTime)
        {
            _jumpTimer = 0;
        }
    }

    private void Heal()
    {
        if (RemainingHeal <= 0 || Math.Abs(HealthPoints - _statSystem.PlayerStat.HealthPoints) < 0.1) return;

        RemainingHeal--;
        HealthPoints += _statSystem.PlayerStat.HealingAmount;
        HealthPoints = Mathf.Min(HealthPoints, _statSystem.PlayerStat.HealthPoints);
    }

    public void Attack()
    {
        var bolt = (Bolt) _bolt.Instance();
        bolt.Damage = _statSystem.PlayerStat.DamageDealt;

        bolt.Shoot(Position + 32 * FacingDirection, FacingDirection);

        GetParent().AddChild(bolt);
    }

    public void Dash()
    {
        // You can only dash if you're not already dashing, are on the ground and can dash 
        if (_isDashing /*|| !HaveJumpsLeft*/ || !_canDash) return;

        _isDashing = true;
        _canDash = false;
        Vulnerable = false;
        //_jumps++;
        RunAfterDelay(() =>
            {
                _isDashing = false;
                Vulnerable = true;
            }, DashDuration)
            .ThenAfterDelay(() => _canDash = true, DashCoolDown);
    }

    public void Hit(float damage, Vector2 source)
    {
        if (!Vulnerable) return;

        // _knockingBack is a force that's applied after the movement and gradually decreases over time
        var knockDirection = Position > source ? 1 : -1;
        _knockingBack = new Vector2(knockDirection, -0.5f) * KnockbackForce;

        Vulnerable = false;
        RunAfterDelay(() => Vulnerable = true, InvicibilityCoolDown);
        
        HealthPoints -= damage;

        if (HealthPoints <= 0)
        {
            Die();
        }

        _animations.Play("hurt");
    }

    public void Hit(float damage, Node2D source) => Hit(damage, source.Position);

    private void Die()
    {
        _knockingBack = Vector2.Zero;
        _vel = Vector2.Zero;
        _statSystem.SpiritCount *= 0.8f;
        
        // Delay just so we can see the hurting animation
        RunAfterDelay(() => _sceneSwitcher.Switch(Scene.Hub, "FROM_DEATH"), 200);
    }
}