using Godot;
using KongrooTools;
using System;
namespace Cleato
{
    public partial class GameManager : Singleton<GameManager>
    {
		public PlayerMaster PlayerMaster = null;
        public override void _EnterTree()
        {
            base._EnterTree();
            GD.Print("Starting GameManager");
        }
    }
}