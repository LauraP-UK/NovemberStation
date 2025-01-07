using System;
using Godot;

public abstract class ControllerBase : Listener {
    private const float
        JUMP_VELOCITY = 6.0f,
        WALK_SPEED = 4.5f,
        SPRINT_SPEED = 7.0f,
        AIR_CAP = 0.85f,
        AIR_ACCELERATION = 800.0f,
        AIR_MOVE_SPEED = 500.0f,
        GROUND_ACCELERATION = 50.0f,
        GROUND_DECELERATION = 10.0f,
        GROUND_FRICTION = 6.0f,
        MAX_STEP_HEIGHT = 0.5f,
        APPROX_ACTOR_MASS = 80.0f,
        MIN_IMPULSE_THRESHOLD = 1.25f;

    private ActorBase _actor { get; }
    private ulong _lastFrameOnFloor = ulong.MinValue;
    private bool _snappedToStairs;
    private Vector3? _savedCamGlobalPosition = null;

    protected Vector3 _intendedDirection = Vector3.Zero;
    protected bool _sprinting = false, _jumping, _crouching = false;

    protected ControllerBase(ActorBase actor) {
        _actor = actor;
        _actor.SetController(this);
    }

    protected float GetSpeed() => _sprinting ? SPRINT_SPEED : WALK_SPEED;
    protected abstract void OnUpdate(float delta);

    private void SaveCameraPosition() {
        if (GetActor() is not IViewable actor) return;
        _savedCamGlobalPosition ??= actor.GetCamera().GlobalPosition;
    }

    private void SlideCamToOrigin(float delta) {
        if (_savedCamGlobalPosition == null || GetActor() is not IViewable actor) return;
        Camera3D camera = actor.GetCamera();
        
        Vector3 globalPosition = camera.GetGlobalPosition();
        globalPosition.Y = _savedCamGlobalPosition.Value.Y;
        camera.SetGlobalPosition(globalPosition);
        
        Vector3 position = camera.GetPosition();
        position.Y = Mathf.Clamp(position.Y, -0.7f, 0.7f);
        camera.SetPosition(position);

        float moveAmount = Math.Max(GetActor().GetModel().Velocity.Length() * delta, WALK_SPEED / 2 * delta);
        position.Y = Mathf.MoveToward(position.Y, 0.0f, moveAmount);
        camera.SetPosition(position);
        _savedCamGlobalPosition = camera.GlobalPosition;
        
        if (position.Y == 0.0f) _savedCamGlobalPosition = null;
    }
    
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

        velocity.Y -= (float)ProjectSettings.GetSetting("physics/3d/default_gravity") * delta;
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
            _lastFrameOnFloor = Engine.GetPhysicsFrames();
            Vector3 velocity = model.GetVelocity();
            velocity.Y = _jumping ? JUMP_VELOCITY : 0.0f;
            model.SetVelocity(velocity);
            HandleGroundPhysics(delta);
        }
        else HandleAirPhysics(delta);

        PushAwayRigidBodies();
        if (!SnapUpStairsCheck(delta)) {
            model.MoveAndSlide();
            SnapToStairsCheck();
        }

        SlideCamToOrigin(delta);
        
        _intendedDirection = Vector3.Zero;
        _jumping = false;
    }

    protected bool IsSurfaceTooSteep(Vector3 normal) => normal.AngleTo(Vector3.Up) > GetActor().GetModel().FloorMaxAngle;

    protected bool RunBodyTestMotion(Transform3D from, Vector3 motion, PhysicsTestMotionResult3D result = null) {
        result ??= new PhysicsTestMotionResult3D();
        PhysicsTestMotionParameters3D physicsParams = new();

        physicsParams.From = from;
        physicsParams.Motion = motion;

        return PhysicsServer3D.BodyTestMotion(GetActor().GetModel().GetRid(), physicsParams, result);
    }

    private bool SnapUpStairsCheck(float delta) {
        CharacterBody3D model = GetActor().GetModel();
        if (!model.IsOnFloor() && !_snappedToStairs)
            return false;

        Vector3 expectedMove = model.Velocity * new Vector3(1.0f, 0.0f, 1.0f) * delta;
        Transform3D stepPosClearance = model.GlobalTransform.Translated(expectedMove + new Vector3(0, MAX_STEP_HEIGHT * 2, 0));

        PhysicsTestMotionResult3D downCheckResult = new();

        if (RunBodyTestMotion(stepPosClearance, new Vector3(0.0f, -MAX_STEP_HEIGHT * 2.0f, 0.0f), downCheckResult) &&
            (downCheckResult.GetCollider() is StaticBody3D || downCheckResult.GetCollider() is CsgShape3D)) {
            float stepHeight = (stepPosClearance.Origin + downCheckResult.GetTravel() - model.GlobalPosition).Y;

            if (stepHeight > MAX_STEP_HEIGHT ||
                stepHeight <= 0.01f ||
                (downCheckResult.GetCollisionPoint() - model.GlobalPosition).Y > MAX_STEP_HEIGHT)
                return false;

            RayCast3D frontCast = GetActor().GetFrontRaycast();
            frontCast.SetGlobalPosition(downCheckResult.GetCollisionPoint() + new Vector3(0, MAX_STEP_HEIGHT, 0) + expectedMove.Normalized() * 0.1f);
            frontCast.ForceRaycastUpdate();

            if (!frontCast.IsColliding() || IsSurfaceTooSteep(frontCast.GetCollisionNormal()))
                return false;

            SaveCameraPosition();
            
            model.SetGlobalPosition(stepPosClearance.Origin + downCheckResult.GetTravel());
            model.ApplyFloorSnap();
            _snappedToStairs = true;
            return true;
        }

        return false;
    }

    private void SnapToStairsCheck() {
        CharacterBody3D model = GetActor().GetModel();
        RayCast3D belowCast = GetActor().GetBelowRaycast();

        bool didSnap = false;
        bool floorBelow = belowCast.IsColliding() && !IsSurfaceTooSteep(belowCast.GetCollisionNormal());
        bool wasOnFloor = Engine.GetPhysicsFrames() - _lastFrameOnFloor == 1;

        if (!model.IsOnFloor() && model.GetVelocity().Y <= 0 && (wasOnFloor || _snappedToStairs) && floorBelow) {
            PhysicsTestMotionResult3D result = new();
            if (RunBodyTestMotion(model.GlobalTransform, new Vector3(0.0f, -MAX_STEP_HEIGHT, 0.0f), result)) {
                
                SaveCameraPosition();
                
                float transY = result.GetTravel().Y;
                Vector3 position = model.GetPosition();
                model.SetPosition(new Vector3(position.X, position.Y += transY, position.Z));
                model.ApplyFloorSnap();
                didSnap = true;
            }
        }

        _snappedToStairs = didSnap;
    }

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