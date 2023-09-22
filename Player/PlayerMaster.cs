using Cleato;
using Godot;
using System;

public partial class PlayerMaster : Node
{
	private Player _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("../Player");
		GameManager._.PlayerMaster = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
