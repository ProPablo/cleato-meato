using System.Collections.Generic;
using Godot;

public partial class MeatCloth : Area3D
{
    [Export] public Godot.Collections.Array<Node3D> StickPoints;
    private MeatSoftBody _meatSoftBody;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _meatSoftBody = GetNode<MeatSoftBody>("SoftBody");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        foreach (var node in StickPoints)
        {
            // Move the x position of each node closer to the center
            var pos = node.Position;
            pos.X = Mathf.Lerp(pos.X, 0, 0.1f * (float)delta);
            node.Position = pos;
        }
        if (Input.IsActionJustPressed("grapple"))
        {
            _meatSoftBody.ReleasePoints();
            foreach (var node in StickPoints)
            {
                node.QueueFree();
            }
            StickPoints.Clear();
        }
    }
}
