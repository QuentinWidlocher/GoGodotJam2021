using System;
using Godot;

public class GeneratorButton : HBoxContainer
{
	private bool _initialized;
	private Generator _generator = null!;
	public Generator Generator
	{
		get => _generator;
		set { 
			_generator = value;
			
			if (_initialized)
				UpdateValues();
		}
	}

	private Label _costLabel = null!;
	private Label _boughtLabel = null!;
	private Button _button = null!;
	
	public override void _Ready()
	{
		_costLabel = GetNode<Label>("CostLabel");
		_boughtLabel = GetNode<Label>("BoughtLabel");
		_button = GetNode<Button>("Button");

		_initialized = true;
		
		UpdateValues();
	}

	public void UpdateValues()
	{
		_costLabel.Text = $"Cost : {Math.Round(Generator.Cost, 2)} Spirit";
		_boughtLabel.Text = $"Bought : {Generator.Bought}";
		_button.Text = Generator.Name;
		_button.Disabled = !IdleSystem.CanAffordGenerator(Generator);
	}

	public void OnBuyGenerator()
	{
		IdleSystem.BuyGenerator(Generator.Id);
	}
}