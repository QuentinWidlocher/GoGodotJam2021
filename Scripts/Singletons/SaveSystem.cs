using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using static Helpers.NodeExtensions;
using static Helpers.TaskHelpers;
using Object = System.Object;

public class SaveSystem : Node
{
	private SceneSwitcher _sceneSwitcher = null!;
	private Player _player = null!;
	private StatSystem _statSystem = null!;

	private string _savePath = "user://save.dat";
	
	public override void _Ready()
	{
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		_player = GetNode<Player>("/root/Player");
		_statSystem = GetNode<StatSystem>("/root/StatSystem");
	}

	public void Save()
	{
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
			["SpiritAmount"] = _statSystem.SpiritCount,
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
			var storedPosition = (Vector2) saveData["PlayerPosition"];
			var tileMap = GetTree().CurrentScene.FindInChildren<TileMap>();

			if (tileMap != null)
			{
				var tilePosition = tileMap.MapToWorld(tileMap.WorldToMap(storedPosition));
				var topOfTile = tilePosition + (tileMap.CellSize / 2);
				_player.Position = topOfTile;
			}
			else
			{
				_player.Position = storedPosition;
			}
			
			_player.RemainingHeal = (int) saveData["PlayerHeal"];
			_player.HealthPoints = (float) saveData["PlayerHealth"];
			_statSystem.SpiritCount = (float) saveData["SpiritAmount"];
			_sceneSwitcher.CurrentScene = (Scene)((int) saveData["CurrentScene"]);
			
			var camera = _player.FindInChildren<Camera2D>();
			if (camera != null)
			{
				camera.SmoothingEnabled = false;
				RunAfterDelay(() => camera.SmoothingEnabled = true, 100);
			}

			foreach (var enemy in GetTree().CurrentScene.GetChildren<Enemy>())
			{
				try
				{
					Dictionary infos = (Dictionary)((Dictionary) saveData["EnemiesInfos"])[enemy.GetPath()];
					
					enemy._healthPoints = (float) infos["EnemiesHealth"];
					enemy.MaxHealthPoints = (float) infos["EnemiesMaxHealth"];
					enemy.Position = (Vector2) infos["EnemiesPosition"];
				}
				catch (KeyNotFoundException e)
				{
					enemy.QueueFree();
				}
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