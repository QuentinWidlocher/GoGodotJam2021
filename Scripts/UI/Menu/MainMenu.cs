using Godot;

public class MainMenu : Control
{

	private Button _continueButton = null!;
	private SceneSwitcher _sceneSwitcher = null!;

	public override void _Ready()
	{
		_continueButton = GetNode<Button>("ContinueButton");
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		
		_continueButton.GrabFocus();
	}

	public void OnContinueButtonPressed()
	{
		_sceneSwitcher.GoToScene(Scene.Hub);
	}
	
	public void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}