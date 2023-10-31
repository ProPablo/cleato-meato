using Godot;
using KongrooTools;
using System;
using System.Diagnostics.Contracts;
namespace Cleato
{
    public partial class GameManager : Singleton<GameManager>
    {
        public PlayerMaster PlayerMaster = null;

        public GlobalState GlobalState = null;

        public int CurrentDay = 0;

        public DayRes CurrentDayRes => GlobalState.Days[CurrentDay];
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

        public void GotoScene(PackedScene scene)
        {
            // This function will usually be called from a signal callback,
            // or some other function from the current scene.
            // Deleting the current scene at this point is
            // a bad idea, because it may still be executing code.
            // This will result in a crash or unexpected behavior.

            // The solution is to defer the load to a later time, when
            // we can be sure that no code from the current scene is running:

            CallDeferred(MethodName.DeferredGotoScene, scene);
        }

        public void DeferredGotoScene(PackedScene scene)
        {
            // It is now safe to remove the current scene
            GetTree().CurrentScene.Free();

            // Instance the new scene.
            var currentScene = scene.Instantiate();

            // Add it to the active scene, as child of root.
            GetTree().Root.AddChild(currentScene);

            // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
            GetTree().CurrentScene = currentScene;
        }

        private void DebugCommands()
        {
            if (Input.IsKeyPressed(Key.R))
            {
                GetTree().ReloadCurrentScene();
            }

            if (Input.IsKeyPressed(Key.Escape))
            {
                GetTree().Quit();
            }

            if (Input.IsKeyPressed(Key.F1))
            {
                DialogManager._.PlayDialog(GlobalState.Days[0].Dialog);
            }

            if (Input.IsKeyPressed(Key.F2))
            {
                //Progres the level 
                ProgressLevel();
            }
        }

        public void ProgressLevel()
        {
            CurrentDay++;
            GotoScene(this.GlobalState.HubScene);

        }
    }
}