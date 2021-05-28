using System;
using Godot;

public class VaniaUI : Control
{
	[Export] private NodePath PlayerPath = null!;

	private ProgressBar _hpBar = null!;
	private Player _player = null!;

	private float _valueToAimFor;

	public override void _Ready()
	{
		_player = GetNode<Player>(PlayerPath);
		_player.Connect(nameof(Player.HealthChange), this, nameof(OnPlayerHpChange));
		
		_hpBar = GetNode<ProgressBar>("Healthbar/ProgressBar");
		_hpBar.MaxValue = StatSystem.PlayerStat.HealthPoints;
		_valueToAimFor = _player.HealthPoints;
	}

	public override void _Process(float delta)
	{
		if (Math.Abs(_hpBar.Value - _valueToAimFor) > _hpBar.Step)
		{
			_hpBar.Value = Mathf.Lerp((float) _hpBar.Value, _valueToAimFor, 0.5f);
		}
	}

	private void OnPlayerHpChange(float newValue)
	{
		_valueToAimFor = newValue;
	}
}