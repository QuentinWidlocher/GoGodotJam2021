using Godot;

public class JumpingEnemy : RigidDetectionEnemy
{
    [Export] public float JumpForce = 100;
    [Export] public float JumpEveryXSec = 1;
    [Export] public override float Damage { get; set; }
    [Export] public new float MaxHealthPoints = 3f;

    private Timer _timer = null!;
    private RayCast2D _floorCast1 = null!;
    private RayCast2D _floorCast2 = null!;
    private AnimatedSprite _sprite = null!;
    private AudioStreamPlayer2D _deathSound = null!;

    private readonly float _maxFloorDelay = 0.5f;
    private float _floorDelay;

    private bool _preparingJump;

    private bool OnGround => _floorCast1.IsColliding() || _floorCast2.IsColliding();

    public override void _Ready()
    {
        base._Ready();

        _healthPoints = MaxHealthPoints;

        _timer = GetNode<Timer>("Timer");
        _floorCast1 = GetNode<RayCast2D>("FloorCast1");
        _floorCast2 = GetNode<RayCast2D>("FloorCast2");

        _timer.OneShot = true;
        _timer.Connect("timeout", this, nameof(Jump));

        _sprite = (AnimatedSprite) GetNode("Sprite");

        _deathSound = (AudioStreamPlayer2D) GetNode("DeathSound");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        // We prepare to jump only if we're not already preparing to, if the player has been seen and if we're on ground
        if (!_preparingJump && _hasSeenPlayer && _target != null && OnGround)
        {
            PrepareJump();
        }
        
        if (_floorDelay > 0)
        {
            _floorDelay -= delta;

            if (_floorDelay <= 0)
            {
                _floorDelay = 0;
                _floorCast1.Enabled = true;
                _floorCast2.Enabled = true;
            }
        }
    }

    private void PrepareJump()
    {
        _preparingJump = true;
        _sprite.Play("wait");
        _timer.Stop();
        _timer.WaitTime = JumpEveryXSec;
        _timer.Start();
    }

    private void Jump()
    {
        bool aimLeft = _target?.Position.x < Position.x;
        ApplyCentralImpulse(new Vector2(aimLeft ? -1 : 1, -1) * JumpForce);
        _sprite.Play("jump");
        _sprite.FlipH = !aimLeft;
        _preparingJump = false;

        // Prevent the floor detection to triggering at the very start of the jump
        DisableFloorDetection(500);
    }

    private void DisableFloorDetection(int ms)
    {
        _floorCast1.Enabled = false;
        _floorCast2.Enabled = false;
        _floorDelay = _maxFloorDelay;
    }
}