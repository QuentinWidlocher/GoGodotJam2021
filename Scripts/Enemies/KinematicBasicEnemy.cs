using Godot;

public abstract class KinematicBasicEnemy : KinematicBody2D
{
    [Export] public float MaxHealthPoints = 10f;
    private float _healthPoints;

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
    }
}