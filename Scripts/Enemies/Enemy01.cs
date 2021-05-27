using System.Threading.Tasks;
using Godot;
using Helpers;

public class Enemy01 : RigidBasicEnemy
{
    [Export] public float JumpForce = 10;
    [Export] public float JumpEveryXSec = 1;
    [Export] public float Damage = 1; 
    public new float MaxHealthPoints = 2f;

    private Timer _timer = null!;
    private RayCast2D _floorCast = null!;

    private Node2D? _target;
    private bool _preparingJump;

    public override void _Ready()
    {
        base._Ready();
        
        _timer = GetNode<Timer>("Timer");
        _floorCast = GetNode<RayCast2D>("FloorCast");

        _timer.OneShot = true;
        _timer.Connect("timeout", this, nameof(Jump));
    }

    public override void _Process(float delta)
    {
        if (!_preparingJump && _target != null && _floorCast.IsColliding())
        {
            PrepareJump();
        }

        foreach (var body in GetCollidingBodies())
        {
            if (body is Player player)
            {
                player.Hit(Damage, this);
            }
        }
    }

    public void OnBodyTouched(Node body)
    {
        
    }

    public void OnDetectionZoneEntered(Node body)
    {
        if (body is Player target)
        {
            _target = target;
        }
    }

    public void OnDetectionZoneExited(Node body)
    {
        if (body is Player)
        {
            _target = null;
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
        _floorCast.Enabled = false;
        Task.Run(async () =>
        {
            await Task.Delay(ms);
            _floorCast.Enabled = true;
        });
    }
}