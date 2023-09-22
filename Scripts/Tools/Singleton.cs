
using Godot;
using System;
namespace KongrooTools
{
    public partial class Singleton<T> : Node where T : Node
    {
        public static T _ { get; private set; }

        public override void _Ready()
        {
            _ = this as T;
            GD.Print("Running base ready");
        }
    }
}