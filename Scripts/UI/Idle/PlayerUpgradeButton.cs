using System;
using Godot;

public class PlayerUpgradeButton : HBoxContainer
{
	private bool _initialized;
	private PlayerUpgrade _playerUpgrade = null!;
	public PlayerUpgrade PlayerUpgrade
	{
		get => _playerUpgrade;
		set { 
			_playerUpgrade = value;
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
		_costLabel.Text = $"Cost : {Math.Round(PlayerUpgrade.Cost, 2)} Mana";
		_boughtLabel.Text = $"Bought : {PlayerUpgrade.Bought}";
		_button.Text = PlayerUpgrade.Name;
		_button.Disabled = !StatSystem.CanAffordPlayerUpgrade(PlayerUpgrade);
	}

	public void OnBuyPlayerUpgrade()
	{
		StatSystem.BuyPlayerUpgrade(PlayerUpgrade.Id);
	}
}