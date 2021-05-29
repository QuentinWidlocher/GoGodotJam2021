using Godot;

public abstract class KinematicBasicEnemy : KinematicBody2D, Enemy
{
    [Export] public float MaxHealthPoints { get; set; }
    public float _healthPoints { get; set; }

    public override void _Ready()
    {
        MaxHealthPoints = 10;
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
    }
}