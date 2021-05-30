using Godot;

public class SpiritOrb : Particles2D
{
	private Player? _target;
	public int Value = 1;

	public override void _Process(float delta)
	{
		if (_target != null)
		{
			GlobalPosition = GlobalPosition.LinearInterpolate(_target.GlobalPosition, delta);
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
		if (body is Player player)
		{
			StatSystem.SpiritCount += Value;
			QueueFree();
		}
	}
}