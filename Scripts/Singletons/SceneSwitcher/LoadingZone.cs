using System;
using Godot;

public class LoadingZone : Area2D
{
	[Export] public readonly Scene ToScene;
	[Export] public readonly string? FromId;
	[Export] public string Id = "FROM_X_TO_Y_#1";

	public Position2D Spawn = null!;

	private SceneSwitcher _sceneSwitcher = null!;

	public override void _Ready()
	{
		Spawn = GetNode<Position2D>("Spawn");
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		Connect("body_entered", this, nameof(OnBodyEntered));
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Player)
		{
			_sceneSwitcher.Switch(ToScene, this);
		}
	}
}