using Cleato;
using Godot;
using System;

public partial class Credits : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GameManager._.IsInIntro = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override async void _Process(double delta)
    {
		// Quick timer lole 
        await ToSignal(GetTree().CreateTimer(3), "timeout");
        if (Input.IsActionPressed("attack"))
        {
            GetTree().Quit();
        }
    }
}
