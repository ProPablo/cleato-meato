using Godot;
using System;
using System.Diagnostics;

public partial class SkyboxColour : WorldEnvironment
{


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Environment.BackgroundColor = new Color(255,255,255,1);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    private void ChangeSkyboxColour(string buttonname)
    {
        Environment.BackgroundColor = new Color(0.62f, 0.2f, 0.2f);
    }


}
