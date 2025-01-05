
using System;
using Godot;

public abstract class ControllerBase : Listener {
    private const float
        JUMP_VELOCITY = 6.0f, WALK_SPEED = 5.0f, SPRINT_SPEED = 7.0f,
        AIR_CAP = 0.85f, AIR_ACCELERATION = 800.0f, AIR_MOVE_SPEED = 500.0f,
        GROUND_ACCELERATION = 50.0f, GROUND_DECELERATION = 10.0f, GROUND_FRICTION = 6.0f;
    
    private ActorBase _actor { get; }
    protected Vector3 _intendedDirection = Vector3.Zero;

    protected bool _sprinting = false, _jumping;

    private const float MAX_SPEED = 4.0f;

    protected ControllerBase(ActorBase actor) {
        _actor = actor;
        _actor.SetController(this);
    }
    
    protected float GetSpeed() => _sprinting ? SPRINT_SPEED : WALK_SPEED;

    protected abstract void OnUpdate(float delta);

    protected void HandleGroundPhysics(float delta) {
        CharacterBody3D model = GetActor().GetModel();
        float speed = GetSpeed();
        Vector3 velocity = model.GetVelocity();

        float curSpeedInDirection = velocity.Dot(_intendedDirection);
        float addSpeedTillCap = speed - curSpeedInDirection;

        if (addSpeedTillCap > 0.0f) {
            float accelSpeed = GROUND_ACCELERATION * delta * speed;
            accelSpeed = Math.Min(accelSpeed, addSpeedTillCap);
            model.SetVelocity(velocity + accelSpeed * _intendedDirection);
        }
        
        velocity = model.GetVelocity();
        float velLength = velocity.Length();
        float control = Math.Max(velLength, GROUND_DECELERATION);
        float drop = control * GROUND_FRICTION * delta;
        float newSpeed = Math.Max(velLength - drop, 0.0f);

        if (velLength > 0.0f) newSpeed /= velLength;
        model.SetVelocity(velocity * newSpeed);
    }

    protected void HandleAirPhysics(float delta) {
        CharacterBody3D model = GetActor().GetModel();
        Vector3 velocity = VectorUtils.RoundTo(model.GetVelocity(), 4);
        
        velocity.Y -= (float) ProjectSettings.GetSetting("physics/3d/default_gravity") * delta;
        model.SetVelocity(velocity);

        float currentSpeedInDirection = velocity.Dot(_intendedDirection);
        float cappedSpeed = Math.Min((AIR_MOVE_SPEED * _intendedDirection).Length(), AIR_CAP);
        float addSpeedTillCap = cappedSpeed - currentSpeedInDirection;

        if (addSpeedTillCap > 0.0f) {
            float accelSpeed = AIR_ACCELERATION * AIR_MOVE_SPEED * delta;
            accelSpeed = Math.Min(accelSpeed, addSpeedTillCap);
            model.SetVelocity(model.GetVelocity() + accelSpeed * _intendedDirection);
        }
    }

    public void Update(float delta) {
        OnUpdate(delta);

        CharacterBody3D model = GetActor().GetModel();
        if (model.IsOnFloor()) {
            Vector3 velocity = model.GetVelocity();
            velocity.Y = _jumping ? JUMP_VELOCITY : 0.0f;
            model.SetVelocity(velocity);
            HandleGroundPhysics(delta);
        }
        else HandleAirPhysics(delta);

        PushAwayRigidBodies();
        model.MoveAndSlide();
        
        _intendedDirection = Vector3.Zero;
        _jumping = false;
    }

    private const float APPROX_ACTOR_MASS = 80.0f;
    private const float MIN_IMPULSE_THRESHOLD = 1.25f;
    
    private void PushAwayRigidBodies() {
        CharacterBody3D model = GetActor().GetModel();
        for (int i = 0; i < model.GetSlideCollisionCount(); i++) {
            KinematicCollision3D collision = model.GetSlideCollision(i);
            if (collision.GetCollider() is RigidBody3D rigidBody) {
                float massRatio = Math.Min(1.0f, APPROX_ACTOR_MASS / rigidBody.Mass);

                Vector3 pushDirection = -collision.GetNormal();
                pushDirection.Y = 0.0f;
                pushDirection = pushDirection.Normalized();
                
                float velocityDiff = model.GetVelocity().Dot(pushDirection) - rigidBody.LinearVelocity.Dot(pushDirection);
                velocityDiff = Math.Max(0.0f, velocityDiff);
                
                if (velocityDiff < MIN_IMPULSE_THRESHOLD) continue;
                
                float pushForce = massRatio * 1.0f; // Magic adjustment factor
                
                Vector3 impulse = VectorUtils.RoundTo(pushDirection * velocityDiff * pushForce, 4);
                Vector3 position = collision.GetPosition() - rigidBody.GetGlobalPosition();
                position.Y = 0.0f;
                
                rigidBody.ApplyImpulse(impulse, position);
            }
        }
    }
    
    protected ActorBase GetActor() => _actor;
}