using System;
using Godot;
using Range = Godot.Range;

public class VaniaUI : Control
{
	private Range _hpBar = null!;
	private Range _healBar = null!;
	private Player _player = null!;

	private float _hpValueToAimFor;

	public override void _Ready()
	{
		_player = GetNode<Player>("/root/Player");
		_player.Connect(nameof(Player.HealthChange), this, nameof(OnPlayerHpChange));
		
		_hpBar = GetNode<Range>("HealthBar/ProgressBar");
		_hpBar.MaxValue = StatSystem.PlayerStat.HealthPoints;
		_hpValueToAimFor = _player.HealthPoints;
		
		_healBar = GetNode<Range>("HealthBar/HealBar");
		_healBar.Value = _player.RemainingHeal;
	}

	public override void _Process(float delta)
	{
		if (Math.Abs(_hpBar.Value - _hpValueToAimFor) > _hpBar.Step)
		{
			_hpBar.Value = Mathf.Lerp((float) _hpBar.Value, _hpValueToAimFor, 0.5f);
		}
	}

	private void OnPlayerHpChange(float newValue)
	{
		_hpValueToAimFor = newValue;
		_healBar.Value = _player.RemainingHeal;
	}
}