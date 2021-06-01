using Godot;

public class HelpScreen : CanvasLayer
{
	private SceneSwitcher _sceneSwitcher = null!;
	private Player _player = null!;
	
	public override void _Ready()
	{
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		_player = GetNode<Player>("/root/Player");
		
		_player.Enabled = false;
		
	}
	
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("pause"))
			_sceneSwitcher.Switch(Scene.MainMenu);
	}
}