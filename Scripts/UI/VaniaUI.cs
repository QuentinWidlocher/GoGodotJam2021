using System;
using Godot;
using Range = Godot.Range;

public class VaniaUI : Control
{
	private Range _hpBar = null!;
	private Range _healBar = null!;
	private Player _player = null!;
	private Label _spirit = null!;
	private StatSystem _statSystem = null!;

	private float _hpValueToAimFor;

	public override void _Ready()
	{
		_statSystem = GetNode<StatSystem>("/root/StatSystem");
		
		_player = GetNode<Player>("/root/Player");
		_player.Connect(nameof(Player.HealthChange), this, nameof(OnPlayerHpChange));
		
		
		_hpBar = GetNode<Range>("HealthBar/ProgressBar");
		_hpBar.MaxValue = _statSystem.PlayerStat.HealthPoints;
		_hpValueToAimFor = _player.HealthPoints;
		
		_healBar = GetNode<Range>("HealBar");
		_healBar.Value = _player.RemainingHeal;

		_spirit = GetNode<Label>("Spirit/Label");
		_spirit.Text = $"{Math.Round(_statSystem.SpiritCount, 2)}";
		_statSystem.Connect(nameof(StatSystem.SpiritChange), this, nameof(OnSpiritChange));
		_statSystem.Connect(nameof(StatSystem.MaxHealthChange), this, nameof(OnMaxHpChange));
	}

	public override void _Process(float delta)
	{
		if (Math.Abs(_hpBar.Value - _hpValueToAimFor) > _hpBar.Step)
		{
			_hpBar.Value = Mathf.Lerp((float) _hpBar.Value, _hpValueToAimFor, 0.5f);
			GetNode<Label>("HealthBar/Amount").Text = $"{_hpValueToAimFor}/{_statSystem.PlayerStat.HealthPoints}";
		}
	}

	private void OnPlayerHpChange(float newValue)
	{
		_hpValueToAimFor = newValue;
		_healBar.Value = _player.RemainingHeal;
	}

	private void OnMaxHpChange()
	{
		GetNode<Label>("HealthBar/Amount").Text = $"{_hpValueToAimFor}/{_statSystem.PlayerStat.HealthPoints}";
	}

	private void OnSpiritChange(float newValue)
	{
		_spirit.Text = $"{Math.Round(newValue, 2)}";
	}
}