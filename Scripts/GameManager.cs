using Godot;
using KongrooTools;
using System;
namespace Cleato
{
    public partial class GameManager : Singleton<GameManager>
    {
		public PlayerMaster PlayerMaster = null;

        public GlobalState GlobalState = null;
        // Save Dialog scene here

        public override void _EnterTree()
        {
            base._EnterTree();
            GD.Print("Starting GameManager");
            GlobalState = ResourceLoader.Load<GlobalState>("res://GlobalState.tres");
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            #if DEBUG
            DebugCommands();
            #endif
        }

        private void DebugCommands()
        {
            if (Input.IsKeyPressed(Key.R))
            {
                GetTree().ReloadCurrentScene();
            }

            // if (Input.IsKeyPressed(Key.Escape))
            // {
            //     GetTree().Quit();
            // }

            if (Input.IsKeyPressed(Key.F1))
            {
                DialogManager._.PlayDialog(GlobalState.Days[0].Dialog);
            }
        }
    }
}