using Godot;
using Godot.Collections;

public class SaveSystem : Node
{
	private SceneSwitcher _sceneSwitcher = null!;
	private Player _player = null!;

	private string _savePath = "user://save.dat";
	
	public override void _Ready()
	{
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		_player = GetNode<Player>("/root/Player");
	}

	public void Save()
	{
		GD.Print("SAVE | ", _player.Position);
		
		var saveData = new Dictionary<string, object>
		{
			["PlayerPosition"] = _player.Position,
			["PlayerHealth"] = _player.HealthPoints,
			["PlayerHeal"] = _player.RemainingHeal,
			["CurrentScene"] = _sceneSwitcher.CurrentScene ?? Scene.Hub	,
		};

		using File file = new File();
		
		file.Open(_savePath, File.ModeFlags.Write);
		file.StoreVar(saveData, true);
	}

	public void Load()
	{
		using File file = new File();

		if (!file.FileExists(_savePath)) return;
		
		file.Open(_savePath, File.ModeFlags.Read);

		var saveData = (Dictionary?) file.GetVar(true);

		if (saveData != null)
		{
			GD.Print("LOAD | ", saveData["PlayerPosition"]);
			_player.Position = (Vector2) saveData["PlayerPosition"] + new Vector2(0, -50);
			_player.HealthPoints = (float) saveData["PlayerHealth"];
			_player.RemainingHeal = (int) saveData["PlayerHeal"];
			_sceneSwitcher.CurrentScene = (Scene)((int) saveData["CurrentScene"]);
		}
	}
}