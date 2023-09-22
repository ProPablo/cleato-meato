using Godot;
using KongrooTools;
using System;
namespace Cleato
{
    public partial class GameManager : Singleton<GameManager>
    {
		public PlayerMaster PlayerMaster = null;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            GD.Print("Starting GameManager");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }
    }
}