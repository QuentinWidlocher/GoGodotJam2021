using Godot;

public class ShowInEditorOnly : Node2D
{
	public override void _Ready()
	{
		Visible = false;
	}
}