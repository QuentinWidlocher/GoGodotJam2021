using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Helpers.NodeExtensions;

public class FlyingEnemy : KinematicDetectionEnemy
{
    [Export] public new float Damage = 1;
    [Export] public float Speed = 1;

    private List<RayCast2D> _restDetectionCasts = new List<RayCast2D>();

    private Vector2? _restTarget;
    private bool _resting;
    private readonly Vector2[] _restCastsDirections = {
        new Vector2(-1, -1),
        new Vector2(-0.5f, -1f),
        new Vector2(0, -1),
        new Vector2(0.5f, -1f),
        new Vector2(1, -1),
    };

    private int _restSearchTime;
    
    public new float MaxHealthPoints = 2f;

    public override void _Ready()
    {
        base._Ready();

        _restDetectionCasts = GetNode<Node2D>("RestDetectionCasts").GetChildren<RayCast2D>().ToList();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (PlayerIsKnow)
        {
            GD.Print("GoToPlayer");
            _resting = false;
            _restTarget = null;
            GoToThePlayer();
        }
        else if (_restTarget == null && !_resting)
        {
            GD.Print("FindSpotToRest");
            FindSpotToRest();
        }
        else if (!_resting && _restTarget != null)
        {
            GD.Print("Moving To Spot");
            
            foreach (var cast in _restDetectionCasts)
            {
                cast.Enabled = false;
                cast.CastTo = Vector2.Zero;
                _restSearchTime = 0;
            }

            var direction = _restTarget.Value.Normalized();
            MoveAndSlide(direction * Speed);
            
            if (GetSlideCount() > 0)
            {
                _restTarget = null;
                _resting = true;
            }
        }
        else
        {
            GD.Print("Resting");
            _restTarget = null;
            _resting = true;
        }
    }

    private void GoToThePlayer()
    {
        var direction = (_target!.GlobalPosition - GlobalPosition).Normalized();

        if (Math.Abs(GlobalPosition.y - _target!.GlobalPosition.y) > 10)
        {
            direction.y = GlobalPosition.y < _target.GlobalPosition.y ? 1 : -1;
        }

        MoveAndSlide(direction * new Vector2(Speed, Speed / 2), Vector2.Up);
    }

    private void FindSpotToRest()
    {
        for (int i = 0; i < _restDetectionCasts.Count(); i++)
        {
            _restDetectionCasts[i].Enabled = true;
            _restDetectionCasts[i].CastTo = _restCastsDirections[i] * _restSearchTime;

            if (_restDetectionCasts[i].IsColliding())
            {
                if (_restDetectionCasts[i].GetCollider() is StaticBody2D)
                {
                    _restTarget = _restDetectionCasts[i].CastTo;
                    break;
                }
            }
            
            _restSearchTime += 2;
        }
    }
}