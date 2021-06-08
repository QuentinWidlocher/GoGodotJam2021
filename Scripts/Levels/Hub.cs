using Godot;
using static ServiceLocator;

public class Hub : Node2D
{
	public override void _Ready()
	{
		PlayerInstance.RemainingHeal = StatSystemService.PlayerStat.MaxHeals;
		PlayerInstance.HealthPoints = StatSystemService.PlayerStat.HealthPoints;

	}

	public void OpenTreeMenu(Node body)
	{
		if (body ! is Player player) return;
		
		GD.Print("TODO : Open the idle system");
	}
}