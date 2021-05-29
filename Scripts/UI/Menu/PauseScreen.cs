using Godot;

public class PauseScreen : Control
{
	private bool _gamePaused;

	private Button _continueButton = null!;
	
	public override void _Ready()
	{
		_continueButton = GetNode<Button>("ContinueButton");
		Pause(false);	
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("pause"))
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
		GetTree().Quit();
	}
}