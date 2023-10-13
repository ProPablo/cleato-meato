using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody3D
{
    [Export] public float MaxSpeed = 10f;
    [Export] public float JumpImpulse = 4.5f;
    [Export] public float Acceleration = 10f;
    [Export] public Curve TurnAroundCurve;
    [Export] public float AirControl = 0.1f;
    [Export] public float MouseSenstivity = 0.25f;

    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public PlayerMaster Master;
    private Node3D _cameraPivot;

    private Vector3 _desiredUnitDir = Vector3.Zero;
    private bool _jumpRequested = false;

    public override void _EnterTree()
    {
        base._EnterTree();
        Master = GetNode<PlayerMaster>("PlayerMaster");
    }

    public override void _Ready()
    {
        _cameraPivot = GetNode<Node3D>("Pivot");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        _desiredUnitDir = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (Input.IsActionJustPressed("jump"))
            _jumpRequested = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        Movement(delta);
    }

    ///<summary>
    /// Moves the player based on the input and the physics process delta.
    ///</summary>
    private void Movement(double delta)
    {
        Vector3 target = Velocity;

        target.X = _desiredUnitDir.X * MaxSpeed;
        target.Z = _desiredUnitDir.Z * MaxSpeed;

        // Remap the dot product to a value from 0 to 1
        var targetDot = target.Dot(Velocity) + 1 / 0.5f;
        var accel = Acceleration * TurnAroundCurve.Sample(targetDot);
        GD.Print($"player accel: {accel}");

        var moveDir = Velocity.MoveToward(target, accel* (float)delta);

        // Add the gravity.
        if (!IsOnFloor())
            moveDir.Y = target.Y - Gravity * (float)delta;

        // Handle Jump.
        if (_jumpRequested && IsOnFloor()) {
            moveDir.Y = JumpImpulse;
            _jumpRequested = false;
        }

        Velocity = moveDir;
        MoveAndSlide();
    }

    /// <summary>
    /// Applies an instantaneous force impulse to the player (add velocity)
    /// </summary>
    /// <param name="force">The force impulse to apply.</param>
    public void ApplyForceImpulse(Vector3 force)
    {
        Velocity += force;
    }

    private void OldMovement(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= Gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && IsOnFloor())
            velocity.Y = JumpImpulse;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * MaxSpeed;
            velocity.Z = direction.Z * MaxSpeed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, MaxSpeed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, MaxSpeed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

}
