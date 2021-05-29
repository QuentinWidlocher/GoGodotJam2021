using Godot;

public class RigidDetectionEnemy: RigidBasicEnemy
{
    [Export] public float Damage = 1; 
    
    protected bool _hasSeenPlayer;
    protected Node2D? _target;
    
    protected RayCast2D _playerCast = null!;

    protected bool PlayerIsKnow => _target != null && _hasSeenPlayer;

    public override void _Ready()
    {
        base._Ready();
        
        _playerCast = GetNode<RayCast2D>("PlayerCast");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        // We search for the player only if he's in range, but we did not already see him
        if (!_hasSeenPlayer && _target != null)
        {
            _playerCast.CastTo = Transform.XformInv(_target.GlobalPosition); // == seen from Transform
            if (_playerCast.GetCollider() is Player)
            {
                _hasSeenPlayer = true;
            }
        }
        
        foreach (var body in GetCollidingBodies())
        {
            if (body is Player player)
            {
                player.Hit(Damage, this);
            }
        }
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
            // If the player leaves the detection area, we reset our knowledge of him
            _target = null;
            _hasSeenPlayer = false;
        }
    }
}