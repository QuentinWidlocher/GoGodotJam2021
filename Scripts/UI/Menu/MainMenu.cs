using Godot;

public class MainMenu : Control
{

	private Button _continueButton = null!;
	private SceneSwitcher _sceneSwitcher = null!;
	private SaveSystem _saveSystem = null!;

	private bool _alreadyLoadedOnce;
	
	public override void _Ready()
	{
		_continueButton = GetNode<Button>("ContinueButton");
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		_saveSystem = GetNode<SaveSystem>("/root/SaveSystem");
		
		_continueButton.GrabFocus();


		if (_alreadyLoadedOnce)
		{
			_saveSystem.Save();
		}
	}

	public void OnContinueButtonPressed()
	{
		_saveSystem.Load();
		_alreadyLoadedOnce = true;
		
		// Since the scene switcher has still not changed, CurrentScene is either null or = to the current scene when saved
		var nextScene = _sceneSwitcher.CurrentScene ?? Scene.Hub;
		var defaultLoadZone = nextScene == Scene.Hub ? "TO_HUB_1" : null;
		_sceneSwitcher.Switch(nextScene, defaultLoadZone);
	}
	
	public void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}