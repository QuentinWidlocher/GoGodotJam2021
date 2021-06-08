using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Helpers.NodeExtensions;
using static ServiceLocator;

public class IdleMenu : CanvasLayer
{
	[Export] private NodePath GeneratorsPath = null!;
	[Export] private NodePath PlayerUpgradesPath = null!;
	[Export] private NodePath TotalSpiritPath = null!;
	[Export] private NodePath TotalManaPath = null!;
	[Export] private NodePath ManaProductionPath = null!;
	
	private Label _totalSpiritLabel = null!;
	private Label _totalManaLabel = null!;
	private Label _manaProductionLabel = null!;

	private readonly PackedScene _playerUpgradeButton = GD.Load<PackedScene>("res://Scenes/UI/Idle/PlayerUpgradeButton.tscn");
	private readonly List<PlayerUpgradeButton> _playerUpgradeButtons = new List<PlayerUpgradeButton>();
	private readonly PackedScene _generatorButton = GD.Load<PackedScene>("res://Scenes/UI/Idle/GeneratorButton.tscn");
	private readonly List<GeneratorButton> _generatorButtons = new List<GeneratorButton>();

	private float PreviousCurrency;
	
	public override void _Ready()
	{
		PlayerInstance.Enabled = false;
		
		_totalSpiritLabel = GetNode<Label>(TotalSpiritPath);
		_totalManaLabel = GetNode<Label>(TotalManaPath);
		_manaProductionLabel = GetNode<Label>(ManaProductionPath);

		foreach (var generatorButton in IdleSystem.Generators)
		{
			var newGenerator = (GeneratorButton) _generatorButton.Instance();
			newGenerator.Generator = generatorButton;
			GetNode(GeneratorsPath).AddChild(newGenerator);
			_generatorButtons.Add(newGenerator);
		}
		
		foreach (var playerUpgrade in StatSystem.PlayerUpgrades)
		{
			var newButton = (PlayerUpgradeButton) _playerUpgradeButton.Instance();
			newButton.PlayerUpgrade = playerUpgrade;
			GetNode(PlayerUpgradesPath).AddChild(newButton);
			_playerUpgradeButtons.Add(newButton);
		}

		((Node) GetNode(GeneratorsPath).GetChildren()[0]).GetNode<Button>("Button").GrabFocus();
	}

	public override void _Process(float delta)
	{
		if (StatSystemService.GameEnded)
		{
			PlayerInstance.Enabled = false;
			SceneSwitcherService.Switch(Scene.GameEnd);
		}
		
		if (Input.IsActionJustPressed("pause"))
		{
			PlayerInstance.Enabled = true;
			SceneSwitcherService.Switch(Scene.Hub, "HUB_TO_IDLE_1");
		}
		
		// Rider wants me to perform math to gain accuracy, but I don't want accuracy, I want speed !
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (PreviousCurrency == IdleSystem.Currency) return;
		
		PreviousCurrency = IdleSystem.Currency;

		_totalSpiritLabel.Text = $"{Math.Round(StatSystemService.SpiritCount, 2)}";
		
		_totalManaLabel.Text = $"{Math.Round(IdleSystem.Currency, 2)}";
		_manaProductionLabel.Text = $"{Math.Round(IdleSystem.Production, 2)}/s";
		
		_generatorButtons.ForEach(button => button.UpdateValues());
		_playerUpgradeButtons.ForEach(button => button.UpdateValues());
	}
}