
using System;
using Godot;

public abstract class ControllerBase : Listener {
    
    private ActorBase _actor { get; }
    protected Vector3 _velocityInfluence = Vector3.Zero;

    private const float MAX_SPEED = 2.0f;

    protected ControllerBase(ActorBase actor) {
        _actor = actor;
        _actor.SetController(this);
    }

    public void Update(float delta) {
        CharacterBody3D model = GetActor().GetModel();

        if (!_velocityInfluence.Equals(Vector3.Zero)) GD.Print($"INFO: ControllerBase.Update() : _velocityInfluence = {_velocityInfluence}");
        
        Vector3 vel = model.Velocity + _velocityInfluence;
        
        if (!model.IsOnFloor()) vel.Y += -9.8f * delta;
        
        vel.X *= 0.85f;
        vel.Z *= 0.85f;

        vel.X = Math.Clamp(vel.X, -MAX_SPEED, MAX_SPEED);
        vel.Z = Math.Clamp(vel.Z, -MAX_SPEED, MAX_SPEED);
        
        model.Velocity = vel;
        _velocityInfluence = Vector3.Zero;
        
        PushAwayRigidBodies();
        
        model.MoveAndSlide();
    }

    private const float APPROX_ACTOR_MASS = 80.0f;
    
    private void PushAwayRigidBodies() {
        CharacterBody3D model = GetActor().GetModel();
        for (int i = 0; i < model.GetSlideCollisionCount(); i++) {
            KinematicCollision3D collision = model.GetSlideCollision(i);
            if (collision.GetCollider() is RigidBody3D rigidBody) {
                Vector3 pushDirection = -collision.GetNormal();
                float velocityDiff = model.GetVelocity().Dot(pushDirection) - rigidBody.LinearVelocity.Dot(pushDirection);
                velocityDiff = Math.Max(0.0f, velocityDiff);
                float massRatio = Math.Min(1.0f, APPROX_ACTOR_MASS / rigidBody.Mass);

                pushDirection.Y *= 0.25f;
                float pushForce = massRatio * 1.0f; // Magic adjustment factor
                rigidBody.ApplyImpulse(pushDirection * velocityDiff * pushForce, collision.GetPosition() - rigidBody.GetGlobalPosition());
            }
        }
    }
    
    protected ActorBase GetActor() => _actor;
}