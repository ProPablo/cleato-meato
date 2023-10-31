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
        public DayRes CurrentDayRes => (CurrentDay >= 0) ? GlobalState.Days[CurrentDay] : null;
        public MeatRes CurrentMeat;
        public float CurrentTime = float.MaxValue;
        public bool IsInHub = false;
        public bool IsInIntro = true;
    

        public override void _EnterTree()
        {
            base._EnterTree();
            GD.Print("Starting GameManager");
            GlobalState = ResourceLoader.Load<GlobalState>("res://GlobalState.tres");
            CurrentMeat = new MeatRes() { Tier1 = 0, Tier2 = 0 };
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (!IsInHub)
            {
                CurrentTime -= (float)delta;
                if (CurrentTime <= 0)
                {
                    CurrentTime = float.MaxValue;
                    GD.Print("You Lose!");
                    LoserBoy();
                }
            }

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
            // This is not IsKeyJustPressed, so watch out for repeat input
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
                BackToHub();
            }
            if (Input.IsKeyPressed(Key.F3))
            {
                LoserBoy();
            }

        }

        public void CheckWinCondition()
        {
            var tier1Delta = CurrentDayRes.MeatRequirements.Tier1 - CurrentMeat.Tier1;
            var tier2Delta = CurrentDayRes.MeatRequirements.Tier2 - CurrentMeat.Tier2;
            if (tier1Delta <= 0 && tier2Delta <= 0)
            {
                GD.Print("You Win!");
                BackToHub();
            }
            else
            {
                var lines = new Godot.Collections.Array<string>(
                new string[] { "You don't have enough meat to progress",
                    $"You need {tier1Delta} more GreyCon and {tier2Delta} more Choice Cut",
                    "Now get back to work and get me some meat!"
                });

                var notEnoughMeatDialog = new DialogRes()
                {
                    Lines = lines
                };

                DialogManager._.PlayDialog(notEnoughMeatDialog);
            }
        }

        public void GoToNextLevel()
        {
            var nextLevel = CurrentDayRes.LevelScene;
            CurrentMeat = new MeatRes() { Tier1 = 0, Tier2 = 0 };
            CurrentTime = CurrentDayRes.MaxTime;
            IsInHub = false;
            GotoScene(nextLevel);
        }

        public void BackToHub()
        {
            //Signifies the end of the day
            CurrentDay++;
            IsInHub = true;
            GotoScene(GlobalState.HubScene);
        }

        public void ResetGame()
        {
            CurrentDay = 0;
            CurrentMeat = new MeatRes() { Tier1 = 0, Tier2 = 0 };
            CurrentTime = CurrentDayRes.MaxTime;
            IsInHub = true;
            GotoScene(GlobalState.HubScene);
        }

        public void LoserBoy()
        {
            IsInHub = true;
            DialogManager._.PlayDialog(GlobalState.LoserDialog);
            DialogManager._.DialogEnded += () =>
            {
                IsInHub = true;
                GotoScene(GlobalState.HubScene);
            };

        }
    }
}