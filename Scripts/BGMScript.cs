using Cleato;
using Godot;
using System;

public partial class BGMScript : AudioStreamPlayer3D
{
    [Export] public AudioStreamPlayer3D Cave;
    [Export] public AudioStreamPlayer3D Hub;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        CheckIfHub();
    }


    public void CheckIfHub()
    {
        bool placeholder = GameManager._.IsInHub;
        GD.Print(Cave.Playing);
        if (Hub.Playing != placeholder) Hub.Playing = placeholder;
        if (Cave.Playing == placeholder) Cave.Playing = !placeholder;




    }
}
