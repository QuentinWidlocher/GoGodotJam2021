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
		var nextScene = _saveSystem.GetSavedLastScene();
		_alreadyLoadedOnce = true;
		
		// Since the scene switcher has still not changed, CurrentScene is either null or = to the current scene when saved
		var defaultLoadZone = nextScene == Scene.Hub ? "TO_HUB_1" : null;
		_sceneSwitcher.Switch(nextScene, defaultLoadZone);
		_saveSystem.CallDeferred(nameof(SaveSystem.Load));
	}
	
	public void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}