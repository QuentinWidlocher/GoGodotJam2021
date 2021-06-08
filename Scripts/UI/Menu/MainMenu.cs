using Godot;
using static ServiceLocator;

public class MainMenu : Control
{

	private Button _continueButton = null!;

	private bool _alreadyLoadedOnce;
	
	public override void _Ready()
	{
		_continueButton = GetNode<Button>("ContinueButton");

		PlayerInstance.Enabled = false;

		_continueButton.GrabFocus();


		if (_alreadyLoadedOnce)
		{
			SaveSystemService.Save();
		}
	}

	public void OnContinueButtonPressed()
	{
		var nextScene = SaveSystemService.GetSavedLastScene();
		_alreadyLoadedOnce = true;
		PlayerInstance.Enabled = true;
		
		// Since the scene switcher has still not changed, CurrentScene is either null or = to the current scene when saved
		var defaultLoadZone = nextScene == Scene.Hub ? "FROM_DEATH" : null;
		SceneSwitcherService.Switch(nextScene, defaultLoadZone);
		SaveSystemService.CallDeferred(nameof(SaveSystem.Load));
	}

	public void OnGuideButtonPressed()
	{
		SceneSwitcherService.Switch(Scene.Guide);
	}
	
	public void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}