using System;
using Godot;
using static ServiceLocator;

public class LoadingZone : Area2D
{
	[Export] public readonly Scene ToScene;
	[Export] public readonly string? ToId;
	[Export] public string Id = "X_TO_Y_1";

	public Position2D Spawn = null!;

	public override void _Ready()
	{
		Spawn = GetNode<Position2D>("Spawn");
		Connect("body_entered", this, nameof(OnBodyEntered));
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Player)
		{
			SceneSwitcherService.Switch(ToScene, ToId);
		}
	}
}