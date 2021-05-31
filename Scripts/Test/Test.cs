using System;
using System.Diagnostics;
using Godot;
using static GeneratorId;

public class Test : Node
{
	private RichTextLabel ValueLabel = null!;
	private RichTextLabel DeltaLabel = null!;
	
	private Button Generator1Button = null!;
	private Button Upgrade1Button = null!;
	
	private Button Generator2Button = null!;
	private Button Upgrade2Button = null!;

	private float PreviousCurrency;
		
	public override void _Ready()
	{
		ValueLabel = GetNode<RichTextLabel>("Value");
		DeltaLabel = GetNode<RichTextLabel>("Delta");
		
		Generator1Button = GetNode<Button>("Generator1");
		Upgrade1Button = GetNode<Button>("Upgrade1");
		
		Generator2Button = GetNode<Button>("Generator2");
		Upgrade2Button = GetNode<Button>("Upgrade2");

		UpdateButtonLabel(Generator01, Generator1Button, Upgrade1Button);
		UpdateButtonLabel(Generator02, Generator2Button, Upgrade2Button);
	}

	public override void _Process(float delta)
	{
		// Rider wants me to perform math to gain accuracy, but I don't want accuracy, I want speed !
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (PreviousCurrency == IdleSystem.Currency) return;

		PreviousCurrency = IdleSystem.Currency;
		
		ValueLabel.Text = $"Total : {Math.Round(IdleSystem.Currency, 2)} Mana";
		DeltaLabel.Text = $"Production : {Math.Round(IdleSystem.Production, 2)} Mana/s";

		Generator1Button.Disabled = !IdleSystem.CanAffordGenerator(Generator01);
		Upgrade1Button.Disabled = !IdleSystem.CanAffordUpgrade(Generator01);
		
		Generator2Button.Disabled = !IdleSystem.CanAffordGenerator(Generator02);
		Upgrade2Button.Disabled = !IdleSystem.CanAffordUpgrade(Generator02);
	}

	public void OnGenerator1Pressed()
	{
		IdleSystem.BuyGenerator(Generator01);
		UpdateButtonLabel(Generator01, Generator1Button, Upgrade1Button);
	}
	
	public void OnUpgrade1Pressed()
	{
		IdleSystem.BuyUpgrade(Generator01);
		UpdateButtonLabel(Generator01, Generator1Button, Upgrade1Button);
	}
	
	public void OnGenerator2Pressed()
	{
		IdleSystem.BuyGenerator(Generator02);
		UpdateButtonLabel(Generator02, Generator2Button, Upgrade2Button);
	}
	
	public void OnUpgrade2Pressed()
	{
		IdleSystem.BuyUpgrade(Generator02);
		UpdateButtonLabel(Generator02, Generator2Button, Upgrade2Button);
	}

	private static void UpdateButtonLabel(GeneratorId id, Button button, Button upgradeButton)
	{
		var generator = IdleSystem.GetGenerator(id);
		if (generator != null)
		{
			button.Text = $"Buy {generator.Name} ({generator.Bought}) : {Math.Round(generator.Cost, 2)} Mana";
			upgradeButton.Text = $"Buy {generator.Name} Upgrade : {Math.Round(generator.UpgradeCost, 2)} Mana";
		}
	}
}