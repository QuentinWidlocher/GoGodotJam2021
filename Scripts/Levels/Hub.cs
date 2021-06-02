using Godot;

public class Hub : Node2D
{
	private Player _player = null!;
	private StatSystem _statSystem = null!;
	private SceneSwitcher _sceneSwitcher = null!;

	public override void _Ready()
	{
		_player = GetNode<Player>("/root/Player");
		_statSystem = GetNode<StatSystem>("/root/StatSystem");
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");

		_player.RemainingHeal = _statSystem.PlayerStat.MaxHeals;
		_player.HealthPoints = _statSystem.PlayerStat.HealthPoints;

		if (_statSystem.GameEnded)
		{
			_player.Enabled = false;
			_sceneSwitcher.Switch(Scene.GameEnd);
		}
			
	}

	public void OpenTreeMenu(Node body)
	{
		if (body ! is Player player) return;
		
		GD.Print("TODO : Open the idle system");
	}
}