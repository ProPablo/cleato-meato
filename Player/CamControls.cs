using Godot;
using System;
using System.Diagnostics;

// TODO rename to camcontrols
public partial class CamControls : Node3D
{
    [Export] public float Angle = 30f;	//Angle in degrees, gets converted later
    [Export] public float Distance = 5f;
    Node3D _cameraTransform;
    RayCast3D _cameraRay;

    public float upperBounds = 70f;
    public float lowerBounds = 80f;

    private Player _parent;

    public override void _Ready()
    {
        _parent = GetParent<Player>();
        _cameraTransform = GetNode<Node3D>("Camera3D");
        _cameraRay = GetNode<RayCast3D>("CameraPositionRay");
    }

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

    // TODO: use tween to interp campostion
    public void UpdateCamera()
    {
        // Get the camera to raycast from player to a wall, and then move the camera to the raycast hitpoint
        float adjDist = Distance;

        float RZpos = Distance * Mathf.Cos(Mathf.DegToRad(Angle));
        float RYpos = Distance * Mathf.Sin(Mathf.DegToRad(Angle));
        _cameraRay.TargetPosition = new Vector3(0, RYpos, RZpos);
        Vector3 raycastHit;
        float Zpos = 0;
        float Ypos = 0;
        if (_cameraRay.IsColliding())
        {
            raycastHit = ToLocal(_cameraRay.GetCollisionPoint());
            Zpos = raycastHit.Z * 0.9f;
            Ypos = raycastHit.Y * 0.9f;
        }
        else
        {
            Zpos = adjDist * Mathf.Cos(Mathf.DegToRad(Angle));
            Ypos = adjDist * Mathf.Sin(Mathf.DegToRad(Angle));
        }


        _cameraTransform.Position = new Vector3(_cameraTransform.Position.X, Ypos, Zpos);
        //cameraTransform.Rotation = Vector3.Zero;

    }

}
