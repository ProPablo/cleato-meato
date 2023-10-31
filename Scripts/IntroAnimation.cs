using Cleato;
using Godot;
using System;
using System.Diagnostics;

public partial class IntroAnimation : AnimationPlayer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void ChangeAnimation(string oldName)
    {
        if (oldName == "Approach")
        {
			AssignedAnimation = "Decend";
			Play();
        }
        if (oldName == "Decend")
        {
            GameManager._.IsInHub = true;
            GetTree().ChangeSceneToFile("res://Scenes/Hub/hub.tscn");

        }
    }


}
