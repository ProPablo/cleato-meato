using System;
using Godot;

public partial class Player : CharacterBody3D
{
    [Export] public float MaxSpeed = 10f;
    [Export] public float JumpImpulse = 4.5f;
    [Export] public float Acceleration = 10f;
    [Export] public Curve TurnAroundCurve;
    [Export] public float AirControl = 1.1f;
    [Export] public float MouseSenstivity = 0.25f;

    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public PlayerMaster Master;
    private Node3D _cameraPivot;

    private Vector3 _desiredUnitDir = Vector3.Zero;
    private bool _jumpRequested = false;

    private AnimationTree animTree;


    public override void _EnterTree()
    {
        base._EnterTree();
        Master = GetNode<PlayerMaster>("PlayerMaster");

    }

    public override void _Ready()
    {
        _cameraPivot = GetNode<Node3D>("Pivot");
        animTree = GetNode<AnimationTree>("Playermodel/AnimationTree");


        
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
        SetAnimation();
    }

    ///<summary>
    /// Moves the player based on the input and the physics process delta.
    ///</summary>
    private void Movement(double delta)
    {

        Vector2 target;
        target.X = _desiredUnitDir.X * MaxSpeed;
        target.Y = _desiredUnitDir.Z * MaxSpeed;
        var currentVel = new Vector2(Velocity.X, Velocity.Z);

        // Remap the dot product to a value from 0 to 1 (has to be normalized to be a value from -1 to 1)
        var targetDot = (target.Normalized().Dot(currentVel.Normalized()) + 1) / 2;

        // This actually does nothing lol, use the turnaround curve instead, also the air acceleration should be lower than regular acceleration
        // TODO: cache Velocity length to reuse in normalizing
        var airControl = IsOnFloor() ? 1.0f : Mathf.Lerp(1.0f, AirControl, 1.0f - targetDot);

        // float accel = Acceleration ;
        float accel = Acceleration * TurnAroundCurve.Sample(targetDot);
        accel *= airControl;
        //GD.Print($"player accel: {accel}, airControl: {airControl}, targetDot: {targetDot}");

        var moveDir = currentVel.MoveToward(target, accel * (float)delta);
        var moveDir3d = new Vector3(moveDir.X, Velocity.Y, moveDir.Y);

        if (!IsOnFloor())
            moveDir3d.Y = Velocity.Y - Gravity * (float)delta;

        // Handle Jump.
        if (_jumpRequested && IsOnFloor())
        {
            moveDir3d.Y = JumpImpulse;
        }
        _jumpRequested = false;

        Velocity = moveDir3d;
        // Using this, there is no friction built in so we would have to make it ourself
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


    private void SetAnimation()
    {
        Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        animTree.Set("parameters/conditions/InAir", !IsOnFloor());
        animTree.Set("parameters/conditions/OnGround", IsOnFloor());
        animTree.Set("parameters/IWR/blend_position", inputDir);
        GD.Print(inputDir);

    }
}
