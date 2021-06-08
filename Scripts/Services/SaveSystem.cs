using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using static Helpers.NodeExtensions;
using static Helpers.TaskHelpers;
using static ServiceLocator;
using Object = System.Object;

public class SaveSystem : Node
{
	private string _savePath = "user://save.dat";

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
		
		var playerUpgrades = new Dictionary();
		foreach (var upgrade in StatSystem.PlayerUpgrades)
		{
			playerUpgrades[upgrade.Id] = upgrade.Bought;
		}
		
		var generators = new Dictionary();
		foreach (var generator in IdleSystem.Generators)
		{
			generators[generator.Id] = generator.Bought;
		}

		var saveData = new Dictionary
		{
			["PlayerPosition"] = PlayerInstance.Position,
			["PlayerHealth"] = PlayerInstance.HealthPoints,
			["PlayerHeal"] = PlayerInstance.RemainingHeal,
			["ManaAmount"] = IdleSystem.Currency,
			["SpiritAmount"] = StatSystemService.SpiritCount,
			["EnemiesInfos"] = enemiesInfos,
			["PlayerUpgrades"] = playerUpgrades,
			["Generators"] = generators,
			["CurrentScene"] = SceneSwitcherService.CurrentScene ?? Scene.Hub,
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
				PlayerInstance.Position = topOfTile;
			}
			else
			{
				PlayerInstance.Position = storedPosition;
			}
			
			PlayerInstance.RemainingHeal = (int) saveData["PlayerHeal"];
			PlayerInstance.HealthPoints = (float) saveData["PlayerHealth"];
			StatSystemService.SpiritCount = (float) saveData["SpiritAmount"];
			IdleSystem.Currency = (float) saveData["ManaAmount"];
			SceneSwitcherService.CurrentScene = (Scene)((int) saveData["CurrentScene"]);
			
			var camera = PlayerInstance.FindInChildren<Camera2D>();
			if (camera != null)
			{
				camera.SmoothingEnabled = false;
				RunAfterDelay(() => camera.SmoothingEnabled = true, 100);
			}

			var enemiesInfos = ((Dictionary) saveData["EnemiesInfos"]);
			foreach (var enemy in GetTree().CurrentScene.GetChildren<Enemy>())
			{
				try
				{
					Dictionary infos = (Dictionary)enemiesInfos[enemy.GetPath()];
					
					enemy._healthPoints = (float) infos["EnemiesHealth"];
					enemy.MaxHealthPoints = (float) infos["EnemiesMaxHealth"];
					enemy.Position = (Vector2) infos["EnemiesPosition"];
				}
				catch (KeyNotFoundException e)
				{
					enemy.QueueFree();
				}
			}

			var playerUpgrades = (Dictionary) saveData["PlayerUpgrades"];
			foreach (var upgrade in StatSystem.PlayerUpgrades)
			{
				try
				{
					upgrade.Bought = (int) playerUpgrades[upgrade.Id];
				}
				catch (Exception e){ GD.PrintErr(e); }
			}
			
			var generators = (Dictionary) saveData["Generators"];
			foreach (var generator in IdleSystem.Generators)
			{
				try
				{
					generator.Bought = (float) generators[generator.Id];
				}
				catch (Exception e){ GD.PrintErr(e); }
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