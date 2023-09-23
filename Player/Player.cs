using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody3D
{
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;

    public float MouseSenstivity = 0.25f;
	public float upperBounds = 70f;
	public float lowerBounds = 80f;
    private Node3D cameraPivot;
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        Initialise();
    }

    public override void _PhysicsProcess(double delta)
    {
        Movement(delta);
    }

    private void Initialise()
    {
        cameraPivot = GetNode<Node3D>("Pivot");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
		//Mouse Controls
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            Rotate(Vector3.Up, -Mathf.DegToRad(eventMouseMotion.Relative.X * MouseSenstivity));
            float desiredRotation = Mathf.Clamp(cameraPivot.Rotation.X - Mathf.DegToRad(eventMouseMotion.Relative.Y * MouseSenstivity),
			-Mathf.DegToRad(upperBounds),
			Mathf.DegToRad(lowerBounds));
            cameraPivot.Rotation = new Vector3(desiredRotation, cameraPivot.Rotation.Y, cameraPivot.Rotation.Z);
        }
    }
    private void Movement(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && IsOnFloor())
            velocity.Y = JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }


}
