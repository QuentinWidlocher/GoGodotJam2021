using Godot;

public class Test : Node
{
	public override void _Ready()
	{
		GD.Print(StatSystem.PlayerStat);
		GD.Print(StatSystem.StatModifiers);
	}
}