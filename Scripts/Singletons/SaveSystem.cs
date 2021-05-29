using System.Linq;
using Godot;
using Godot.Collections;
using static Helpers.NodeExtensions;
using Object = System.Object;

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

		var enemiesInfos = new Dictionary();
		foreach (var enemy in GetTree().GetNodesInGroup("EnemiesToSave").Cast<Enemy>())
		{
			enemiesInfos[enemy.GetPath()] = new Dictionary()
			{
				["EnemiesHealth"] = enemy._healthPoints,
				["EnemiesMaxHealth"] = enemy.MaxHealthPoints,
				["EnemiesPosition"] = enemy.Position,
			};
		}
		
		var saveData = new Dictionary
		{
			["PlayerPosition"] = _player.Position,
			["PlayerHealth"] = _player.HealthPoints,
			["PlayerHeal"] = _player.RemainingHeal,
			["EnemiesInfos"] = enemiesInfos,
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

			foreach (var enemy in GetTree().CurrentScene.GetChildren<Enemy>())
			{
				var infos = (Dictionary)((Dictionary) saveData["EnemiesInfos"])[enemy.GetPath()];
				enemy._healthPoints = (float) infos["EnemiesHealth"];
				enemy.MaxHealthPoints = (float) infos["EnemiesMaxHealth"];
				enemy.Position = (Vector2) infos["EnemiesPosition"];
			}
		}
	}

	public Scene GetSavedLastScene()
	{
		using File file = new File();

		if (!file.FileExists(_savePath)) return Scene.Hub;
		
		file.Open(_savePath, File.ModeFlags.Read);

		var saveData = (Dictionary?) file.GetVar(true);

		return saveData != null ? (Scene) ((int) saveData["CurrentScene"]) : Scene.Hub;
	}
}