using Godot;
using System;
using System.Diagnostics;

// TODO rename to camcontrols
public partial class CamControls : Node3D
{
    [Export] public float angle = 30f;	//Angle in degrees, gets converted later
    [Export] public float distance = 5f;
    // Called when the node enters the scene tree for the first time.
     public float upperBounds = 70f;
    public float lowerBounds = 80f;   

    private Player _parent;

    public override void _Ready()
    {
        _parent = GetParent<Player>();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        UpdateCamera();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            _parent.Rotate(Vector3.Up, -Mathf.DegToRad(eventMouseMotion.Relative.X * _parent.MouseSenstivity));
            float desiredRotation = Mathf.Clamp(Rotation.X - Mathf.DegToRad(eventMouseMotion.Relative.Y * _parent.MouseSenstivity),
            -Mathf.DegToRad(upperBounds),
            Mathf.DegToRad(lowerBounds));
            Rotation = new Vector3(desiredRotation, Rotation.Y, Rotation.Z);
        }
    }

    public void UpdateCamera()
    {
        // This is tragically bad, Im gonna leave office tomorrow because of this 
        // I need to get the camera to raycast from player to a wall, and then move the camera to the raycast hitpoint
        // TODO: use tween to interp campostion
        Node3D cameraTransform = GetNode<Node3D>("Camera3D");
        RayCast3D cameraRay = GetNode<RayCast3D>("RayCast3D");
        float adjDist = distance;

        float RZpos = distance * Mathf.Cos(Mathf.DegToRad(angle));
        float RYpos = distance * Mathf.Sin(Mathf.DegToRad(angle));
        cameraRay.TargetPosition = new Vector3(0, RYpos, RZpos);
        Vector3 raycastHit;
        float Zpos = 0;
        float Ypos = 0;
        if (cameraRay.IsColliding())
        {
            raycastHit = ToLocal(cameraRay.GetCollisionPoint());
			Zpos = raycastHit.Z*0.9f;
			Ypos = raycastHit.Y*0.9f;
        }
        else
        {
            Zpos = adjDist * Mathf.Cos(Mathf.DegToRad(angle));
            Ypos = adjDist * Mathf.Sin(Mathf.DegToRad(angle));
        }


        cameraTransform.Position = new Vector3(cameraTransform.Position.X, Ypos, Zpos);
        //cameraTransform.Rotation = Vector3.Zero;

    }

}
