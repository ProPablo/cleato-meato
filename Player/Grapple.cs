using System.Linq;
using Godot;
using KongrooTools;

public partial class Grapple : Node3D
{

    [Export] public float GrappleAcceleration = 10f;
    [Export] public float EscapePullForceMult = 1.1f;


    private Player _parent;
    private CamControls _camControls;

    private float _lastDistance = 0f;
    private Node3D _grapplePoint = null;
    private bool _grappleRequest = false;
    public override void _Ready()
    {
        _parent = GetParent<Player>();
        _camControls = _parent.Master.Aim;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("grapple"))
        {
            _grappleRequest = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_grappleRequest)
        {
            var spaceState = GetWorld3D().DirectSpaceState;
            // use global coordinates, not local to node
            var query = PhysicsRayQueryParameters3D.Create(_camControls.Transform.Origin, _camControls.Transform.Origin.DirectionTo(_camControls.Transform.Origin + _camControls.Transform.Basis.Z * 1000f));
            query.Exclude = new Godot.Collections.Array<Rid> { _parent.GetRid() };
            var result = spaceState.IntersectRay(query);
            if (result.Count > 0)
            {
                GD.Print("Hit at point: ", result["position"]);

            }

            _grappleRequest = false;


        }
        if (_grapplePoint == null)
            return;
        base._PhysicsProcess(delta);
        var currentDistance = (_grapplePoint.Transform.Origin - _parent.Transform.Origin).Length();
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
}
