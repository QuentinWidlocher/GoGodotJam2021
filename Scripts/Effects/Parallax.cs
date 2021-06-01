using Godot;
using System;

public class Parallax : ParallaxBackground
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export] public float TopCloudSpeed = 100;
    [Export] public float MidCloudSpeed = 40;
    [Export] public float BottomCloudSpeed = 0;

    private ParallaxLayer _topCloud = null!;
    private ParallaxLayer _midCloud = null!;
    private ParallaxLayer _bottomCloud = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _topCloud = (ParallaxLayer)GetNode("Top");
        _midCloud = (ParallaxLayer)GetNode("Mid");
        _bottomCloud = (ParallaxLayer)GetNode("Bottom");
    }

    public override void _Process(float delta) {
        _topCloud.MotionOffset += new Vector2(delta * TopCloudSpeed, 0);
        _midCloud.MotionOffset += new Vector2(delta * MidCloudSpeed, 0);
        _bottomCloud.MotionOffset += new Vector2(delta * BottomCloudSpeed, 0);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
