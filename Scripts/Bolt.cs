using Godot;
using System;

public class Bolt : KinematicBody2D
{
    [Export]
    public float Speed = 500;

    private Vector2 _direction = Vector2.Right;

    public void Shoot(Vector2 pos, Vector2 dir) {
        Position = pos;
        _direction = dir;
    }

    public override void _PhysicsProcess(float delta) {
        var collision = MoveAndCollide(_direction * Speed * delta);
        if (collision != null) {
            if (collision.Collider.HasMethod("Hit")) {
                collision.Collider.Call("Hit");
            }
            QueueFree();
        }
    }

    public void OnVisibilityNotifier2DScreenExited() {
        QueueFree();
    }
}
