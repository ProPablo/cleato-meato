using Godot;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Transactions;

public partial class MeleeSystem : Area3D
{
    [Export]
    CharacterBody3D host;
    float velocity => host.Velocity.Length();
    bool canAttack = true;
    Timer attackDelayTimer;
    Timer hitboxLingerTimer;

    CollisionShape3D hitboxHost;
    BoxShape3D hitboxShape;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Monitoring = false;

        attackDelayTimer = GetNode<Timer>("SwingTimer");
        hitboxLingerTimer = GetNode<Timer>("HitboxLinger");
        hitboxHost = GetNode<CollisionShape3D>("MeleeHitbox");
        hitboxShape = (BoxShape3D)hitboxHost.Shape;



    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        hitboxShape.Size = new Vector3(1, 1, 2 + Mathf.Clamp(velocity, 0, 10));
        AdjustHitbox();
        if (Input.IsActionPressed("attack") && canAttack)
        {
            meleeAttack();
        }

    }

    public void OnCollision(Node3D body)
    {
        Debug.Print("Hit");
    }

    public void AdjustHitbox()
    {
        hitboxHost.Position = new Vector3(hitboxHost.Position.X, hitboxHost.Position.Y, -hitboxShape.Size.Z / 2f);


    }

    public void meleeAttack()
    {
        this.Monitoring = true;
        canAttack = false;
        attackDelayTimer.Start();
        hitboxLingerTimer.Start();
        Debug.Print("I swung");

    }
    public void OnLingerTimeout()
    {
        this.Monitoring = false;
        Debug.Print("My hitbox turned off");
    }
    public void OnMeleeTimeout()
    {
        canAttack = true;
        Debug.Print("I can swing again");
    }
}
