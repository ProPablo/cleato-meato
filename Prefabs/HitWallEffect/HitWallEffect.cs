using Godot;
using System;

public partial class HitWallEffect : Node3D
{
    private Timer timer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer = GetNode<Timer>("DespawnTimer");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }


    public void OnTimeOut()
    {
		this.QueueFree();
    }
}
