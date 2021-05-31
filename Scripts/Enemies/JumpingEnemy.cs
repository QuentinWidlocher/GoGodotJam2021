using System.Threading.Tasks;
using Godot;
using Helpers;
using static Helpers.TaskHelpers;

public class JumpingEnemy : RigidDetectionEnemy
{
    [Export] public float JumpForce = 100;
    [Export] public float JumpEveryXSec = 1;
    [Export] public override float Damage { get; set; }
    [Export] public new float MaxHealthPoints = 3f;

    private Timer _timer = null!;
    private RayCast2D _floorCast1 = null!;
    private RayCast2D _floorCast2 = null!;
    
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
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        // We prepare to jump only if we're not already preparing to, if the player has been seen and if we're on ground
        if (!_preparingJump && _hasSeenPlayer && _target != null && OnGround)
        {
            PrepareJump();
        }
    }

    private void PrepareJump()
    {
        _preparingJump = true;

        _timer.Stop();
        _timer.WaitTime = JumpEveryXSec;
        _timer.Start();
    }

    private void Jump()
    {
        bool aimLeft = _target?.Position.x < Position.x;
        ApplyCentralImpulse(new Vector2(aimLeft ? -1 : 1, -1) * JumpForce);
        _preparingJump = false;

        // Prevent the floor detection to triggering at the very start of the jump
        DisableFloorDetection(500);
    }

    private void DisableFloorDetection(int ms)
    {
        _floorCast1.Enabled = false;
        _floorCast2.Enabled = false;
        RunAfterDelay(() =>
        {
            _floorCast1.Enabled = true;
            _floorCast2.Enabled = true;
        }, ms);
    }
}