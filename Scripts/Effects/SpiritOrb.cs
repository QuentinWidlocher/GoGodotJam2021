using Godot;

public class SpiritOrb : Particles2D
{
	private Player? _target;
	private StatSystem _statSystem = null!;
	private Light2D _light = null!;
	private AudioStreamPlayer2D _pickupSound = null!;
	
	[Export] public int Value = 100;
	public float FadingSpeed = 1f;
	private bool _fading;
	
	private readonly float _maxDestroyDelay = 1f;
	private float _destroyDelay;

	public override void _Ready()
	{
		_statSystem = GetNode<StatSystem>("/root/StatSystem");
		_light = GetNode<Light2D>("Light2D");
		_pickupSound = (AudioStreamPlayer2D)GetNode("Pickup");
	}

	public override void _Process(float delta)
	{
		if (_target != null)
		{
			GlobalPosition = GlobalPosition.LinearInterpolate(_target.GlobalPosition, delta);
		}

		if (_fading)
		{
			_light.Energy -= FadingSpeed;
			_light.Energy = Mathf.Max(_light.Energy, 0);
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

	public void OnBodyDetected(Node body)
	{
		if (body is Player player)
		{
			_target = player;
		}
	}

	public void OnBodyTouched(Node body)
	{
		if (!_fading && body is Player)
		{
			GD.Print($"Gained {Value} spirits");
			_statSystem.SpiritCount += Value * _statSystem.PlayerStat.SpiritMultiplier;
			
			// We let the orb slowly fade before we destroy it
			Emitting = false;
			_fading = true;
			_pickupSound.Play();
			_destroyDelay = _maxDestroyDelay;
		}
	}
}