using System;
using Godot;
using static ServiceLocator;
using Range = Godot.Range;

public class VaniaUI : Control
{
	private Range _hpBar = null!;
	private Range _healBar = null!;
	private Label _spirit = null!;

	private float _hpValueToAimFor;

	public override void _Ready()
	{
		PlayerInstance.Connect(nameof(Player.HealthChange), this, nameof(OnPlayerHpChange));
		
		
		_hpBar = GetNode<Range>("HealthBar/ProgressBar");
		_hpBar.MaxValue = StatSystemService.PlayerStat.HealthPoints;
		_hpValueToAimFor = PlayerInstance.HealthPoints;
		
		_healBar = GetNode<Range>("HealBar");
		_healBar.Value = PlayerInstance.RemainingHeal;

		_spirit = GetNode<Label>("Spirit/Label");
		_spirit.Text = $"{Math.Round(StatSystemService.SpiritCount, 2)}";
		StatSystemService.Connect(nameof(StatSystem.SpiritChange), this, nameof(OnSpiritChange));
		StatSystemService.Connect(nameof(StatSystem.MaxHealthChange), this, nameof(OnMaxHpChange));
	}

	public override void _Process(float delta)
	{
		if (Math.Abs(_hpBar.Value - _hpValueToAimFor) > _hpBar.Step)
		{
			_hpBar.Value = Mathf.Lerp((float) _hpBar.Value, _hpValueToAimFor, 0.5f);
			GetNode<Label>("HealthBar/Amount").Text = $"{_hpValueToAimFor}/{StatSystemService.PlayerStat.HealthPoints}";
		}
	}

	private void OnPlayerHpChange(float newValue)
	{
		_hpValueToAimFor = newValue;
		_healBar.Value = PlayerInstance.RemainingHeal;
	}

	private void OnMaxHpChange()
	{
		GetNode<Label>("HealthBar/Amount").Text = $"{_hpValueToAimFor}/{StatSystemService.PlayerStat.HealthPoints}";
	}

	private void OnSpiritChange(float newValue)
	{
		_spirit.Text = $"{Math.Round(newValue, 2)}";
	}
}