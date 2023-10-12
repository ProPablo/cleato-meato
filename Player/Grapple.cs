using Godot;
using System;

public partial class Grapple : Node3D
{

    private Player _parent;

	private Node3D grapplePoint = null;
	public override void _Ready()
	{
		_parent = GetParent<Player>();
	}

	public override void _Process(double delta)
	{
	}
}
