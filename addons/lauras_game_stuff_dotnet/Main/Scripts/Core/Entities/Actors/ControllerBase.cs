
using System;
using Godot;

public abstract class ControllerBase : Listener {
    
    private ActorBase _actor { get; }

    protected ControllerBase(ActorBase actor) {
        _actor = actor;
        _actor.SetController(this);
    }

    public void Update(float delta) {
        CharacterBody3D model = GetActor().GetModel();
        Vector3 velocity = model.Velocity;
        
        velocity.X *= 0.9f;
        velocity.Z *= 0.9f;
        
        velocity.Y += -9.8f * delta;
        model.Velocity = velocity;
        
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

                //pushDirection.Y = 0;
                float pushForce = massRatio * 1.0f; // Magic adjustment factor
                rigidBody.ApplyImpulse(pushDirection * velocityDiff * pushForce, collision.GetPosition() - rigidBody.GetGlobalPosition());
            }
        }
    }
    
    protected ActorBase GetActor() => _actor;
}