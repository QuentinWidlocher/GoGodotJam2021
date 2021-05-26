using Godot;

public abstract class RigidBasicEnemy : RigidBody2D
{
    [Export] public float MaxHealthPoints = 10f;

    private float _healthPoints;

    public override void _Ready()
    {
        _healthPoints = MaxHealthPoints;
    }

    public void OnHit(float damage)
    {
        GD.Print("Ouch, ", damage, " damage on ", _healthPoints, " hp");
        _healthPoints -= damage;

        if (_healthPoints <= 0)
        {
            Die();   
        }
    }

    public void Die()
    {
        GD.Print("ded");
        QueueFree();
    }
}