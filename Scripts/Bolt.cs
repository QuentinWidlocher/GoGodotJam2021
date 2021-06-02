using Godot;

public class Bolt : KinematicBody2D
{
    [Export] public float Speed = 500;
    [Export] public float Damage = 1;

    private Vector2 _direction = Vector2.Right;
    private Particles2D _particles = null!;
    private Light2D _light = null!;
    private Sprite _sprite = null!;
    private AudioStreamPlayer2D _hitSound = null!;
    
    private readonly float _maxDestroyDelay = 1f;
    private float _destroyDelay;

    public void Shoot(Vector2 pos, Vector2 dir) {
        _particles = (Particles2D)GetNode("Particles2D");
        _sprite = (Sprite)GetNode("Sprite");
        _light = GetNode<Light2D>("Light2D");
        _hitSound = (AudioStreamPlayer2D)GetNode("HitSound");
        
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

            DeleteBolt();
        }
        
        if (_destroyDelay > 0)
        {
            _destroyDelay -= delta;

            if (_destroyDelay <= 0)
            {
                _destroyDelay = 0;
                QueueFree();
            }
        }
    }

    public void OnVisibilityNotifier2DScreenExited()
    {
        QueueFree();
    }

    private void DeleteBolt()
    {
        // We let the bolt slowly fade before we destroy it
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        _hitSound.Play();
        _sprite.Visible = false;
        _particles.Emitting = false;
        _light.Enabled = false;
        _destroyDelay = _maxDestroyDelay;
    }
}
