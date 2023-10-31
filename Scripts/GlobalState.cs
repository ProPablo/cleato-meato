using Godot;
using System;

[GlobalClass]
public partial class GlobalState : Resource
{
    [Export] public Godot.Collections.Array<DayRes> Days;

    [Export] public PackedScene HubScene;
}
