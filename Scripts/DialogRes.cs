using Godot;
using System;

[GlobalClass]
public partial class DialogRes : Resource
{
    [Export] public Godot.Collections.Array<string> Lines;
}
