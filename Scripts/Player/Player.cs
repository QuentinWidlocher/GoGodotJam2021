using Godot;
using System;
using System.Threading.Tasks;
using Helpers;
using static Helpers.TaskHelpers;

public class Player : KinematicBody2D
{
    [Export] public float Gravity = 50f;
    [Export] public float Acceleration = 150f;
    [Export] public float MaxVelocity = 500f;
    [Export] public float Friction = 0.75f;
    [Export] public float JumpVelocity = 900f;
    [Export] public float JumpTime = 0.15f;
    [Export] public float DashForce = 1000f;
    [Export] public int DashDuration = 250;
    [Export] public int DashCoolDown = 500;
    [Export] public float KnockbackForce = 500;
    [Export] public int InvicibilityCoolDown = 1500;

    // MaxJumps is public to change the number of max jumps from other classes (is there a better way to do this?)
    public int MaxJumps = 2;
    public float HealthPoints;

    private Vector2 _vel;
    private int _jumps = 0;
    private float _jumpTimer = 0;
    private readonly PackedScene _bolt = GD.Load<PackedScene>("res://Scenes/Player/Bolt.tscn");
    private bool _isFacingRight = true;
    private bool _isAimingUp = true;
    private bool _hasLanded = false;
    private Vector2 _knockingBack; // over my shouuulder 🎵
    private AnimatedSprite _sprite = null!;
    private AnimationPlayer _animations = null!;

    private bool _isDashing;
    private bool _canDash = true;

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

        HealthPoints = StatSystem.PlayerStat.HealthPoints;
    }

    public override void _PhysicsProcess(float delta)
    {
        GetInput(delta);

        _vel = MoveAndSlide(_vel, Vector2.Up);

        if (IsOnFloor())
        {
            if (!_hasLanded)
            {
                _animations.Play("landing");
            }

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
    }

    public void GetInput(float delta)
    {
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

            if (Input.IsActionJustPressed("dash"))
            {
                Dash();
            }
        }
        else
        {
            _vel = FacingDirection * DashForce;
        }

        if (_knockingBack != Vector2.Zero)
        {
            GD.Print(_knockingBack);
            _vel += _knockingBack;
            _knockingBack = _knockingBack.LinearInterpolate(Vector2.Zero, .5f);
            if (_knockingBack.Length() < 1)
            {
                _knockingBack = Vector2.Zero;
            }
        }

        // Simulating air resistance and friction (WARNING: Extremely difficult partial differential equation being calculated, proceed with caution)
        _vel.x *= Friction;

        // Applying the gravity before jumping so the jump velocity isn't affected by gravity
        _vel.y += Gravity;

        // When the jump button is held, _jumpTimer counts up to only let the player gain y velocity until JumpTime is reached
        if (Input.IsActionJustPressed("jump"))
        {
            if (_jumps < MaxJumps)
            {
                _animations.Stop();
                _animations.Play("jump");
            }
        }

        if (Input.IsActionPressed("jump"))
        {
            _jumpTimer += delta;
            if (_jumpTimer < JumpTime && _jumps < MaxJumps)
            {
                _vel.y = -JumpVelocity;
            }
        }

        // Resetting jump variables once button is released
        if (Input.IsActionJustReleased("jump"))
        {
            _jumps++;
            _jumpTimer = 0;
        }

        // This is just so _jumpTimer is zeroed even if the player hits a ceiling (which would set the timer to JumpTime)
        if (!Input.IsActionPressed("jump") && _jumpTimer >= JumpTime)
        {
            _jumpTimer = 0;
        }
    }

    public void Attack()
    {
        var bolt = (Bolt) _bolt.Instance();
        bolt.Damage = StatSystem.PlayerStat.DamageDealt;

        bolt.Shoot(Position + 32 * FacingDirection, FacingDirection);

        GetParent().AddChild(bolt);
    }

    public void Dash()
    {
        // You can only dash if you're not already dashing, are on the ground and can dash 
        if (_isDashing || !HaveJumpsLeft || !_canDash) return;

        _isDashing = true;
        _canDash = false;
        Vulnerable = false;
        _jumps++;
        RunAfterDelay(() =>
            {
                _isDashing = false;
                Vulnerable = true;
            }, DashDuration)
            .ThenAfterDelay(() => _canDash = true, DashCoolDown);
    }

    public void Hit(float damage, Node2D source)
    {
        if (!Vulnerable) return;

        // _knockingBack is a force that's applied after the movement and gradually decreases over time 
        _knockingBack = (Position - source.Position).Normalized() * KnockbackForce;

        Vulnerable = false;
        RunAfterDelay(() => Vulnerable = true, InvicibilityCoolDown);

        HealthPoints -= damage;

        if (HealthPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
    }
}