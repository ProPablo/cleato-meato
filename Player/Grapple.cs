using System;
using System.Linq;
using Cleato;
using Godot;
using KongrooTools;



public partial class Grapple : Node3D
{
    [Export] PackedScene GrapplePointMesh;

    [ExportGroup("Tf2 grapple props")]
    [Export] public float GrappleSpeed = 15f;

    [ExportGroup("RoR2 grapple props")]
    [Export] public float GrappleAcceleration = 10f;
    [Export] public float EscapePullForceMult = 1.1f;
    // [Export(PropertyHint.Layers3DPhysics)] 
    public uint GrappleableLayer;

    private Player _parent;
    private CamControls _camControls;
    RayCast3D _grappleRay;
    public bool CanHit => _grappleRay.IsColliding();
    private float _lastDistance = 0f;
    private Node3D _grapplePoint = null;
    private InputRequest _inputRequest = InputRequest.None;

    enum InputRequest
    {
        None,
        Grapple,
        Release,
        JumpGrapple,
    }
    public override void _Ready()
    {
        _parent = GetNode<Player>("../..");
        _camControls = GetNode<CamControls>("..");
        _grappleRay = GetNode<RayCast3D>("ForwardRay");
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("grapple"))
        {
            _inputRequest = InputRequest.Grapple;
        }

        if (Input.IsActionJustPressed("jump"))
        {
            _inputRequest = InputRequest.JumpGrapple;
        }

        if (Input.IsActionJustReleased("grapple"))
        {
            _inputRequest = InputRequest.Release;
        }

    }

    public override void _PhysicsProcess(double delta)
    {
        DetectJumpRelease();
        DetectHit();
        DetectRelease();

        // GrappleMovementRoR(delta);
        GrappleMovementtf2(delta);
    }

    private void GrappleMovementRoR(double delta)
    {
        if (_grapplePoint == null)
            return;
        var currentDistance = (_grapplePoint.GlobalPosition - _parent.GlobalPosition).Length();
        var acceleration = GrappleAcceleration;

        // If we are further out than we were last frame, we are escaping, so we want to pull harder
        if (currentDistance < _lastDistance)
            acceleration *= EscapePullForceMult;

        var player2HookDir = (_grapplePoint.Transform.Origin - _parent.Transform.Origin).Normalized();
        var lookDirMult = Utils.Remap(_camControls.GetAimDir().Dot(player2HookDir), -1f, 1f, 1f, 0.0f);
        var pullForce = player2HookDir * lookDirMult * acceleration * (float)delta;

        _parent.ApplyForceImpulse(pullForce);

        _lastDistance = currentDistance;
    }

    private void GrappleMovementtf2(double delta)
    {
        if (_grapplePoint == null)
            return;

        var player2HookDir = (_grapplePoint.Transform.Origin - _parent.Transform.Origin).Normalized();
        _parent.Velocity = player2HookDir * GrappleSpeed;
    }


    private void DetectHit()
    {
        if (_inputRequest == InputRequest.Grapple)
        {
            if (_grappleRay.IsColliding())
            {
                var loc = _grappleRay.GetCollisionPoint();
                _grapplePoint = GrapplePointMesh.Instantiate<MeshInstance3D>();
                GetTree().Root.AddChild(_grapplePoint);
                _grapplePoint.GlobalPosition = loc;
            }

            _inputRequest = InputRequest.None;
        }
    }
    private void DetectRelease()
    {
        if (_inputRequest == InputRequest.Release)
        {
            if (_grapplePoint != null)
            {
                _grapplePoint.QueueFree();
                _grapplePoint = null;

            }
            _inputRequest = InputRequest.None;
        }
    }
    private void DetectJumpRelease()
    {
        if (_grapplePoint == null)
            return;
        if (_inputRequest == InputRequest.JumpGrapple)
        {
            _grapplePoint.QueueFree();
            _grapplePoint = null;

            var vel = _parent.Velocity;
            if (vel.Y > 0)
            {
                vel.Y += _parent.JumpImpulse;
            }
            else
            {
                vel.Y = _parent.JumpImpulse;
            }
            _parent.Velocity = vel;
            _inputRequest = InputRequest.None;
        }
    }

    private void DetectHitOld()
    {
        // if (_grappleRequest)
        {
            //https://www.youtube.com/watch?v=W7fxTvj4oqo
            //https://docs.godotengine.org/en/stable/tutorials/physics/ray-casting.html
            var spaceState = GetWorld3D().DirectSpaceState;
            // use global coordinates, not local to node
            var ray = _camControls.GetAimRay();
            var query = new PhysicsRayQueryParameters3D()
            {
                From = ray.Origin,
                To = ray.Origin + ray.Direction * 1000f,
                CollisionMask = GrappleableLayer,
            };

            var result = spaceState.IntersectRay(query);
            GD.Print($"Sending Ray {ray}");
            if (result.Count > 0)
            {
                GD.Print("Hit at point: ", result["position"]);
            }

            // _grappleRequest = false;
        }
    }
}
