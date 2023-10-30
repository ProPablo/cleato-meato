using Godot;
using KongrooTools;
using System;
using System.Diagnostics;

public partial class CamControls : Node3D
{
    [Export] public float Angle = 30f;	//Angle in degrees, gets converted later
    [Export] public float Distance = 5f;
    Node3D _cameraTransform;
    RayCast3D _cameraRay;

    public float upperBounds = 70f;
    public float lowerBounds = 80f;

    private Player _parent;

    private float _parentRotDeg = 0f;
    private Vector3 _selfRot = Vector3.Zero;

    public override void _Ready()
    {
        _parent = GetParent<Player>();
        _cameraTransform = GetNode<Node3D>("Camera3D");
        _cameraRay = GetNode<RayCast3D>("CameraPositionRay");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        // We arent setting the rotation directly, but rotating so dont need to worry about global vs local
        _parent.Rotate(Vector3.Up, -Mathf.DegToRad(_parentRotDeg));
        _parentRotDeg = 0;

        Rotation = _selfRot;

        //Putting this logic in Update doesnt help at all
        UpdateCamera();
    }


    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Parent rot is for yaw, self rot is for pitch
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            _parentRotDeg = eventMouseMotion.Relative.X * _parent.MouseSenstivity;
            float desiredRotation = Mathf.Clamp(Rotation.X - Mathf.DegToRad(eventMouseMotion.Relative.Y * _parent.MouseSenstivity), -Mathf.DegToRad(upperBounds), Mathf.DegToRad(lowerBounds));
            _selfRot = new Vector3(desiredRotation, Rotation.Y, Rotation.Z);
        }


        // Use this in update instead of Input events
        // _rotation_input += Input.get_action_raw_strength("camera_left") - Input.get_action_raw_strength("camera_right")
        // _tilt_input += Input.get_action_raw_strength("camera_up") - Input.get_action_raw_strength("camera_down")
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

    public Vector3 GetAimDir()
    {
        return -Transform.Basis.Z;
    }

    public Ray GetAimRay()
    {
        return new Ray(ToGlobal(Transform.Origin), GetAimDir());
    }

}


// Another fix to jittering is to set the max fps to double the physics fps
// https://old.reddit.com/r/godot/comments/wh6vw5/is_there_really_no_proper_fix_to_jittering/


//Fix to jittering 
/*
https://www.youtube.com/watch?v=gL0iibY6-Fg&lc=UgyPm1khb4ncDANvzxt4AaABAg
//https://gafferongames.com/post/fix_your_timestep/
//This is a common problem apparently and should be fixed manually
In Godot 4.x, a few changes need to be made for the code to work. Here is the updated code:

extends Node3D
@export var follow_target: NodePath

var target : Node3D
var update = false
var gt_prev : Transform3D
var gt_current : Transform3D

func _ready():
	set_as_top_level(true)
	target = get_node_or_null(follow_target)
	if target == null:
		target = get_parent()
	global_transform = target.global_transform
	
	gt_prev = target.global_transform
	gt_current = target.global_transform
	
func update_transform():
	gt_prev = gt_current
	gt_current = target.global_transform

func _process(_delta):
	if update:
		update_transform()
		update = false
		
	var f = clamp(Engine.get_physics_interpolation_fraction(), 0, 1)
	global_transform = gt_prev.interpolate_with(gt_current, f)
	
func _physics_process(_delta):
	update = true

    */