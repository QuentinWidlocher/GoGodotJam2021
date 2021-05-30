using Godot;

public interface Enemy
{
    public float MaxHealthPoints { get; set; }
    public float _healthPoints { get; set; }
    public Vector2 Position { get; set; }

    public NodePath GetPath();
    public void QueueFree();
}