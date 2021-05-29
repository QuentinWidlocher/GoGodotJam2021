using Godot;

public class Hub : Node2D
{
	private Player _player = null!;

	public override void _Ready()
	{
		_player = GetNode<Player>("/root/Player");

		_player.RemainingHeal = StatSystem.PlayerStat.MaxHeals;
		_player.HealthPoints = StatSystem.PlayerStat.HealthPoints;
	}

	public void OpenTreeMenu(Node body)
	{
		if (body ! is Player player) return;
		
		GD.Print("TODO : Open the idle system");
	}
}