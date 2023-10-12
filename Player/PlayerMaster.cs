using Cleato;
using Godot;
using System;

// Best to keep this as a cs script as gd scripts would mean needing to type cast a lot (but would make for easier development)
public partial class PlayerMaster : Node
{
    public Player Controller;
	public Grapple Grapple;
	public CamControls Aim;	

    public MeleeSystem Melee;
	[Signal] public delegate void PlayerDiedEventHandler();

    public override void _Ready()
    {
        // https://kidscancode.org/godot_recipes/3.x/basics/getting_nodes/
        // Controller = GetNode<Player>("../../Player");
        Controller = GetParent<Player>();
		Grapple = GetNode<Grapple>("../Grapple");
		Aim = GetNode<CamControls>("../Pivot");
        Melee = GetNode<MeleeSystem>("../Pivot/Melee");

        GameManager._.PlayerMaster = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
