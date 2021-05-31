using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static Helpers.NodeExtensions;

public class IdleMenu : CanvasLayer
{
	[Export] private NodePath GeneratorsPath = null!;
	[Export] private NodePath TotalSpiritPath = null!;
	[Export] private NodePath TotalManaPath = null!;
	[Export] private NodePath ManaProductionPath = null!;
	
	private Label _totalSpiritLabel = null!;
	private Label _totalManaLabel = null!;
	private Label _manaProductionLabel = null!;
	private List<GeneratorButton> _generatorButtons = new List<GeneratorButton>();
	
	private static StatSystem _statSystem = null!;
	private SceneSwitcher _sceneSwitcher = null!;
	
	private float PreviousCurrency;
	
	public override void _Ready()
	{
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
		_statSystem = GetNode<StatSystem>("/root/StatSystem");
		
		_totalSpiritLabel = GetNode<Label>(TotalSpiritPath);
		_totalManaLabel = GetNode<Label>(TotalManaPath);
		_manaProductionLabel = GetNode<Label>(ManaProductionPath);

		_generatorButtons = GetNode(GeneratorsPath).GetChildren<GeneratorButton>().ToList();
		
		int generatorIndex = 0;
		foreach (var generatorButton in _generatorButtons)
		{
			generatorButton.Generator = IdleSystem.Generators[generatorIndex];
			generatorIndex++;
		}
		
		_generatorButtons.First().GetNode<Button>("Button").GrabFocus();
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			_sceneSwitcher.Switch(Scene.Hub, "HUB_TO_IDLE_1");
		}
		
		// Rider wants me to perform math to gain accuracy, but I don't want accuracy, I want speed !
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (PreviousCurrency == IdleSystem.Currency) return;
		
		PreviousCurrency = IdleSystem.Currency;

		_totalSpiritLabel.Text = $"{Math.Round(_statSystem.SpiritCount, 2)}";
		
		_totalManaLabel.Text = $"{Math.Round(IdleSystem.Currency, 2)}";
		_manaProductionLabel.Text = $"{Math.Round(IdleSystem.Production, 2)}/s";
		
		_generatorButtons.ForEach(button => button.UpdateValues());
	}
}