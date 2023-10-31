using Cleato;
using Godot;
using KongrooTools;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Transactions;

public partial class MeleeSystem : Area3D
{
    [Export]
    private CharacterBody3D host;
    private float velocity => host.Velocity.Length();
    private bool canAttack = true;
    private Timer attackDelayTimer;
    private Timer hitboxLingerTimer;
    private RayCast3D forwardRay;
    private CollisionShape3D hitboxHost;
    private BoxShape3D hitboxShape;

    //https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_exports.html
    [Export(PropertyHint.Layers3DPhysics)] public uint Tier1MeatLayer;
    [Export(PropertyHint.Layers3DPhysics)] public uint Tier2MeatLayer;
    [Export(PropertyHint.Layers3DPhysics)] public uint DialogueLayer;

    // TODO: Architect this better (Use some kind of dialog script interaction)
    [Export(PropertyHint.Layers3DPhysics)] public uint BackDoorLayer;

    [Export]
    private PackedScene bloodHit;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Monitoring = false;

        attackDelayTimer = GetNode<Timer>("SwingTimer");
        hitboxLingerTimer = GetNode<Timer>("HitboxLinger");
        hitboxHost = GetNode<CollisionShape3D>("MeleeHitbox");
        forwardRay = GetNode<RayCast3D>("ForwardRay");
        hitboxShape = (BoxShape3D)hitboxHost.Shape;

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        AdjustHitbox();
        if (Input.IsActionPressed("attack") && canAttack)
        {
            MeleeAttack();
        }

    }

    // This is actually OnAreaEnter
    public void OnCollision(Area3D body)
    {
        bool spawnBlood = false;
        if (body.HasLayer(Tier1MeatLayer))
        {
            spawnBlood = true;
            GD.Print("Tier 1 meat hit");
            GameManager._.CurrentMeat.Tier1++;
        }
        else if (body.HasLayer(Tier2MeatLayer))
        {
            spawnBlood = true;
            GD.Print("Tier 2 meat hit");
            GameManager._.CurrentMeat.Tier2++;
        }
        else if (body.HasLayer(DialogueLayer))
        {
            GD.Print("Dialogue hit");
            //Launch the Dialog system for the current day
            var dialog = GameManager._.CurrentDayRes.Dialog;
            if (!DialogManager._.IsInDialog)
            {
                DialogManager._.PlayDialog(dialog);
                //Since this is a lambda function, it gets garbage collected along with cleaning up this class/Node, therefore, no need to unsub
                DialogManager._.DialogEnded += () =>
                {
                    GD.Print("Dialog Finished");
                    GameManager._.GoToNextLevel();
                };
            }
        }
        else if (body.HasLayer(BackDoorLayer))
        {
            GameManager._.CheckWinCondition();
        }
        else
        {
            GD.Print("Hit layer: " + body.CollisionLayer);
        }

        if (forwardRay.IsColliding() && spawnBlood)
        {

            GD.Print("Spawn Attempted");
            Vector3 hitPoint;
            hitPoint = forwardRay.GetCollisionPoint();
            HitWallEffect hit = bloodHit.Instantiate<HitWallEffect>();
            body.AddChild(hit);
            hit.GlobalPosition = hitPoint;
            hit.LookAt(hitPoint + forwardRay.GetCollisionNormal(), Vector3.Up);
            hit.Rotate(Vector3.Forward, (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4));

        }

    }



    public void AdjustHitbox()
    {
        hitboxShape.Size = new Vector3(1, 1, 3 + Mathf.Clamp(velocity * 0.1f, 0, 10));
        hitboxHost.Position = new Vector3(hitboxHost.Position.X, hitboxHost.Position.Y, -hitboxShape.Size.Z / 2f);
        forwardRay.TargetPosition = new Vector3(forwardRay.TargetPosition.X, forwardRay.TargetPosition.Y, -hitboxShape.Size.Z);

    }

    public void MeleeAttack()
    {
        this.Monitoring = true;
        canAttack = false;
        attackDelayTimer.Start();
        hitboxLingerTimer.Start();

    }
    public void OnLingerTimeout()
    {
        this.Monitoring = false;
    }
    public void OnMeleeTimeout()
    {
        canAttack = true;
    }
}
