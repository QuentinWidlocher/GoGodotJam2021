using Godot;

public abstract class KinematicBasicEnemy : KinematicBody2D, Enemy
{
    [Export] public float MaxHealthPoints { get; set; }
    public float _healthPoints { get; set; }
    [Export] public int SpiritValue = 1;

    public override void _Ready()
    {
        _healthPoints = MaxHealthPoints;
    }

    public void OnHit(float damage)
    {
        _healthPoints -= damage;

        if (_healthPoints <= 0)
        {
            Die();   
        }
    }

    public void Die()
    {
        QueueFree();

        var orbScene = GD.Load<PackedScene>("res://Scenes/Effects/SpiritOrb.tscn");
        var orb = orbScene.Instance<SpiritOrb>();
        orb.GlobalPosition = GlobalPosition;
        orb.Value = SpiritValue;
        
        GetTree().CurrentScene.AddChild(orb);
    }
}