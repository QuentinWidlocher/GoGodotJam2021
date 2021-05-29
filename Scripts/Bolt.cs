using Godot;
using System;

public class Bolt : KinematicBody2D
{
    [Export] public float Speed = 500;
    [Export] public float Damage = 1;

    private Vector2 _direction = Vector2.Right;
    private Particles2D _particles = null!;
    private Sprite _sprite = null!;

    public void Shoot(Vector2 pos, Vector2 dir) {
        _particles = (Particles2D)GetNode("Particles2D");
        _sprite = (Sprite)GetNode("Sprite");
        _particles.Emitting = true;
        Position = pos;
        _direction = dir;
        if (_direction.x < 0)
            _sprite.FlipH = true;
    }

    public override void _PhysicsProcess(float delta) {
        var collision = MoveAndCollide(_direction * Speed * delta);
        if (collision != null) {
            if (collision.Collider.HasMethod("OnHit")) {
                collision.Collider.Call("OnHit", Damage);
            }
            QueueFree();
        }
    }

    public void OnVisibilityNotifier2DScreenExited() {
        QueueFree();
    }
}
