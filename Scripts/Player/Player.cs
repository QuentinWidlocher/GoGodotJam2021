using Godot;
using System;
using static Helpers.TaskHelpers;
using static ServiceLocator;

public class Player : KinematicBody2D
{
    [Signal]
    public delegate void HealthChange(float newValue);

    public bool IsFacingRight;
    public Vector2 FacingDirection => IsFacingRight ? Vector2.Right : Vector2.Left;

    public float DashCoolDownTimer;

    private bool enabled;

    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            if (!enabled)
                State = PlayerState.Disabled;
        }
    }

    public IPlayerState State = PlayerState.Falling;

    // MaxJumps is public to change the number of max jumps from other classes (is there a better way to do this?)
    public int MaxJumps => StatSystemService.PlayerStat.HasDoubleJump ? 2 : 1;
    public int RemainingHeal;
    public int Jumps;

    private float healthPoints;

    public float HealthPoints
    {
        get => healthPoints;
        set
        {
            healthPoints = value;
            EmitSignal(nameof(HealthChange), value);
        }
    }

    public RayCast2D DownRayCast = null!;

    public Vector2 Velocity = Vector2.Zero;
    private readonly PackedScene boltScene = GD.Load<PackedScene>("res://Scenes/Player/Bolt.tscn");

    public AnimatedSprite Sprite = null!;
    public AnimationPlayer Animations = null!;
    public AudioStreamPlayer JumpSound = null!;
    public AudioStreamPlayer FootstepSound = null!;
    public AudioStreamPlayer ShootSound = null!;
    public AudioStreamPlayer LandingSound = null!;
    public AudioStreamPlayer HitSound = null!;

    public bool OnGround => DownRayCast.IsColliding();

    public bool HaveJumpsLeft => Jumps < MaxJumps;

    private bool vulnerable = true;

    public bool Vulnerable
    {
        get => vulnerable;
        set
        {
            vulnerable = value;
            // Uncheck the player layer and the enemy mask so we can walk through them 
            SetCollisionLayerBit(1, value);
            SetCollisionMaskBit(2, value);

            byte alpha = (byte) (value ? 255 : 255 / 2);
            Modulate = Color.Color8(255, 255, 255, alpha);
        }
    }

    public override void _Ready()
    {
        Sprite = (AnimatedSprite) GetNode("AnimatedSprite");
        Animations = (AnimationPlayer) GetNode("AnimationPlayer");
        DownRayCast = GetNode<RayCast2D>("RayCast2D");

        JumpSound = (AudioStreamPlayer) GetNode("Sounds/Jump");
        FootstepSound = (AudioStreamPlayer) GetNode("Sounds/Footstep");
        ShootSound = (AudioStreamPlayer) GetNode("Sounds/Shoot");
        LandingSound = (AudioStreamPlayer) GetNode("Sounds/Landing");
        HitSound = (AudioStreamPlayer) GetNode("Sounds/Hit");

        HealthPoints = StatSystemService.PlayerStat.HealthPoints;
        RemainingHeal = StatSystemService.PlayerStat.MaxHeals;
    }

    public override void _PhysicsProcess(float delta)
    {
        GetInput(delta);

        State.UpdatePlayer(this, delta);
        var newState = State.UpdateState(this, delta);
        ChangeState(newState);

        Velocity = MoveAndSlide(Velocity, Vector2.Up);

        if (Input.IsActionPressed("attack_main"))
            Sprite.Play("attack");
    }

    public void GetInput(float delta)
    {
        if (Input.IsActionJustPressed("attack_main"))
        {
            Attack();
        }

        if (Input.IsActionJustPressed("heal"))
        {
            Heal();
        }
    }

    private void Heal()
    {
        if (RemainingHeal <= 0 || Math.Abs(HealthPoints - StatSystemService.PlayerStat.HealthPoints) < 0.1) return;

        RemainingHeal--;
        HealthPoints += StatSystemService.PlayerStat.HealingAmount;
        HealthPoints = Mathf.Min(HealthPoints, StatSystemService.PlayerStat.HealthPoints);
    }

    public void Attack()
    {
        var bolt = (Bolt) boltScene.Instance();
        bolt.Damage = StatSystemService.PlayerStat.DamageDealt;

        bolt.Shoot(Position + 32 * FacingDirection, FacingDirection);

        GetParent().AddChild(bolt);
        ShootSound.Play();
    }

    public void Hit(float damage, Vector2 source)
    {
        if (!Vulnerable) return;

        ChangeState(PlayerState.KnockedBack);

        HealthPoints -= damage;

        if (HealthPoints <= 0)
        {
            Die();
        }
    }

    private void ChangeState(IPlayerState newState)
    {
        if (newState != State)
        {
            var oldState = State;

            State.Exit(newState, this);
            State = newState;
            State.Enter(oldState, this);
        }
    }

    public void Hit(float damage, Node2D source) => Hit(damage, source.Position);

    private void Die()
    {
        Velocity = Vector2.Zero;
        StatSystemService.SpiritCount *= 0.75f;

        // Delay just so we can see the hurting animation
        RunAfterDelay(() => SceneSwitcherService.Switch(Scene.Hub, "FROM_DEATH"), 200);
    }
}