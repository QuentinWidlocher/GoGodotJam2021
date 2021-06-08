using Godot;
using static ServiceLocator;

public class HelpScreen : CanvasLayer
{
	public override void _Ready()
	{
		PlayerInstance.Enabled = false;
	}
	
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("pause"))
			SceneSwitcherService.Switch(Scene.MainMenu);
	}
}