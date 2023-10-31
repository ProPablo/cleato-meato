using Godot;
using System;


[GlobalClass]
public partial class DayRes : Resource
{
    [Export] public DialogRes Dialog;

    [Export] public PackedScene LevelScene;

    // [Export] public int Tier1MeatReq = 5;
    // [Export] public int Tier2MeatReq = 5;
    [Export] public MeatRes MeatRequirements;
    public float BestTime = 0f;
    [Export] public float MaxTime = 0f;
}
