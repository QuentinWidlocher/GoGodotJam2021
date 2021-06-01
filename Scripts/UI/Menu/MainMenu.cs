using Godot;

public class MainMenu : Control
{

	private Button _continueButton = null!;
	private SceneSwitcher _sceneSwitcher = null!;
	private SaveSystem _saveSystem = null!;
	private Player _player = null!;

	private bool _alreadyLoadedOnce;
	
	public override void _Ready()
	{
		_continueButton = GetNode<Button>("ContinueButton");
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		_saveSystem = GetNode<SaveSystem>("/root/SaveSystem");
		_player = GetNode<Player>("/root/Player");

		_player.Enabled = false;

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
		_player.Enabled = true;
		
		// Since the scene switcher has still not changed, CurrentScene is either null or = to the current scene when saved
		var defaultLoadZone = nextScene == Scene.Hub ? "FROM_DEATH" : null;
		_sceneSwitcher.Switch(nextScene, defaultLoadZone);
		_saveSystem.CallDeferred(nameof(SaveSystem.Load));
	}

	public void OnGuideButtonPressed()
	{
		_sceneSwitcher.Switch(Scene.Guide);
	}
	
	public void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}