using Godot;

public class DrawDebug : Control
{
	[Export] public readonly NodePath? TargetPath;
	[Export] public readonly bool Enabled = true;
	[Export] public readonly float Width = 3f;

	private Player _target = null!;
	private Camera2D _camera = null!;
	
	public override void _Ready()
	{
		_target = GetNode<Player>(TargetPath);
		_camera = _target.GetNode<Camera2D>("Camera2D");
	}

	public override void _Draw()
	{
		if (!Enabled) return;
		
		DrawArrow(_target.GlobalTransform.origin, _target.GlobalTransform.origin);
		// DrawArrow(_target.GlobalPosition, _target._playerCast.CastTo, Colors.IndianRed);
	}
	
	private void DrawArrow(Vector2 start, Vector2 end, Color? color = null)
	{
		var c = color ?? Colors.IndianRed;
		var s = start;
		var e = end;
		DrawLine(s, e, c, Width);
		DrawTriangle(e, s.DirectionTo(e), Width * 2, c);
	}

	public override void _Process(float delta)
	{
		if (!Enabled) return;

		Update();
	}

	private void DrawTriangle(Vector2 pos, Vector2 dir, float size, Color color)
	{
		var a = pos + dir * size;
		var b = pos + dir.Rotated(2 * Mathf.Pi / 3) * size;
		var c = pos + dir.Rotated(4 * Mathf.Pi / 3) * size;
		DrawPolygon(new[] {a, b, c}, new[] {color});
	}
}