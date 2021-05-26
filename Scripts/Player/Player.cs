using Godot;
using System;

public class Player : KinematicBody2D
{
    [Export]
    public float Gravity = 50;
    [Export]
    public float Acceleration = 150;
    [Export]
    public float MaxVelocity = 500;
    [Export]
    public float Friction = 0.75f;
    [Export]
    public float JumpVelocity = 900;
    [Export]
    public float JumpTime = 0.15f;
    
    // MaxJumps is public to change the number of max jumps from other classes (is there a better way to do this?)
    public int MaxJumps = 2;

    private Vector2 _vel;
    private int _jumps = 0;
    private float _jumpTimer = 0;
    private readonly PackedScene _bolt = GD.Load<PackedScene>("res://Scenes/Player/Bolt.tscn");
    private bool _isFacingRight = true;
    private bool _isAimingUp = true;


    public override void _PhysicsProcess(float delta) {
        GetInput(delta);
        
        _vel = MoveAndSlide(_vel, Vector2.Up);

        if (IsOnFloor()) {
            _jumps = 0;
        }
        // Preventing the player from continuing their jump when bonking on a ceiling
        if (IsOnCeiling()) {
            _jumpTimer = JumpTime;
        }
    }

    public void GetInput(float delta)
    {
        if (Input.IsActionPressed("left")) {
            // Applying acceleration
            _vel.x -= Input.GetActionStrength("left") * Acceleration;
            // Clamping _vel.x only inside the input event because that way only limit the max velocity caused by input
            // This means that we can go past the maximum velocity if we apply acceleration elsewhere (e.g. dashes, cannons etc.)
            Math.Clamp(_vel.x, -MaxVelocity, MaxVelocity);
            _isFacingRight = false;
        }
        if (Input.IsActionPressed("right")) {
            // See above for explanations
            _vel.x += Input.GetActionStrength("right") * Acceleration;
            Math.Clamp(_vel.x, -MaxVelocity, MaxVelocity);
            _isFacingRight = true;
        }

        // Simulating air resistance and friction (WARNING: Extremely difficult partial differential equation being calculated, proceed with caution)
        _vel.x *= Friction;

        if (Input.IsActionJustPressed("attack_main")) {
            Attack();
        }

        // Applying the gravity before jumping so the jump velocity isn't affected by gravity
        _vel.y += Gravity;
        
        // When the jump button is held, _jumpTimer counts up to only let the player gain y velocity until JumpTime is reached
        if (Input.IsActionPressed("jump")) {
            _jumpTimer += delta;
            if (_jumpTimer < JumpTime && _jumps < MaxJumps) {
                _vel.y = -JumpVelocity;
            }
        }
        // Resetting jump variables once button is released
        if (Input.IsActionJustReleased("jump")) {
            _jumps++;
            _jumpTimer = 0;
        }
        // This is just so _jumpTimer is zeroed even if the player hits a ceiling (which would set the timer to JumpTime)
        if (!Input.IsActionPressed("jump") && _jumpTimer >= JumpTime) {
            _jumpTimer = 0;
        }
    }

    public void Attack()
    {
        var bolt = (Bolt)_bolt.Instance();
        bolt.Damage = StatSystem.PlayerStat.DamageDealt;
        
        var facing = _isFacingRight ? Vector2.Right : Vector2.Left;
        bolt.Shoot(Position + 32 * facing, facing);
        
        GetParent().AddChild(bolt);
    }
}
