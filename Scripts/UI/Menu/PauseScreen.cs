using Godot;

public class PauseScreen : Control
{
	private bool _gamePaused;

	private Button _continueButton = null!;
	private SceneSwitcher _sceneSwitcher = null!;
	
	public override void _Ready()
	{
		_continueButton = GetNode<Button>("ContinueButton");
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		
		Pause(false);	
	}

	public override void _Process(float delta)
	{
		if (_sceneSwitcher.CurrentScene != Scene.MainMenu && Input.IsActionJustPressed("pause"))
			TogglePause();
	}

	private void TogglePause()
	{
		Pause(!_gamePaused);
	}

	private void Pause(bool status = true)
	{
		_gamePaused = status;
		GetTree().Paused = status;
		Visible = status;

		if (status)
		{
			_continueButton.GrabFocus();
		}
	}

	public void OnContinueButtonPressed()
	{
		Pause(false);
	}
	
	public void OnExitButtonPressed()
	{
		Pause(false);
		_sceneSwitcher.Switch(Scene.MainMenu);
	}
}