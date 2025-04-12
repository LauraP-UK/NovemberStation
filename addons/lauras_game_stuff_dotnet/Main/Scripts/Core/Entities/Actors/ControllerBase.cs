using System;
using Godot;

public abstract class ControllerBase<T> : Listener, IActorController where T : ActorBase {
    private const float
        JUMP_VELOCITY = 6.0f,
        WALK_SPEED = 4.5f,
        SPRINT_SPEED = 7.0f,
        CROUCH_SPEED = WALK_SPEED * 0.5f,
        INERTIA_GAIN = 0.15f,
        INERTIA_LOSS = 0.075f,
        AIR_CAP = 0.85f,
        AIR_ACCELERATION = 800.0f,
        AIR_MOVE_SPEED = 500.0f,
        GROUND_ACCELERATION = 50.0f,
        GROUND_DECELERATION = 10.0f,
        GROUND_FRICTION = 6.0f,
        MAX_STEP_HEIGHT = 0.5f,
        APPROX_ACTOR_MASS = 80.0f,
        MIN_IMPULSE_THRESHOLD = 1.25f,
        CROUCH_TRANSLATE = 1.075f,
        CROUCH_JUMP_HEIGHT = CROUCH_TRANSLATE * 0.9f;

    private T _actor { get; }
    private ulong _lastFrameOnFloor = ulong.MinValue;
    private bool _snappedToStairs, _lockedMode, _crouchingLastFrame;
    private Vector3? _lastCamGlobalPosition = null;

    protected float _inertia, _leanInertia;
    protected Vector3 _intendedDirection = Vector3.Zero, _lastDirection = Vector3.Zero, _leanTarget = Vector3.Zero;
    protected bool _sprinting = false, _jumping, _crouching, _tryUncrouch, _inputThisFrame;
    protected readonly float _actorHeight = -1.0f;

    protected ControllerBase(T actor) {
        _actor = actor;
        _actor.SetController(this);
        if (GetActor().GetCollisionShape().GetShape() is CapsuleShape3D capsule)
            _actorHeight = capsule.Height;
    }

    protected T GetActor() => _actor;
    protected float GetSpeed() => _crouching ? CROUCH_SPEED : _sprinting ? SPRINT_SPEED : WALK_SPEED;
    public void SetLocked(bool locked) => _lockedMode = locked;
    public bool IsLocked() => _lockedMode;

    protected abstract void OnUpdate(float delta);
    public void Update(float delta) {
        OnUpdate(delta);
    }
    
    protected abstract void OnPhysicsUpdate(float delta);
    public void PhysicsUpdate(float delta) {
        OnPhysicsUpdate(delta);
        CharacterBody3D model = GetActor().GetModel();

        HandleCrouch(delta);
        
        _leanInertia = Math.Clamp(_leanInertia + (_inputThisFrame ? INERTIA_GAIN : -INERTIA_LOSS), 0.0f, 1.0f);
        
        if (model.IsOnFloor()) {
            _lastFrameOnFloor = Engine.GetPhysicsFrames();
            Vector3 velocity = model.GetVelocity();
            velocity.Y = _jumping ? JUMP_VELOCITY : 0.0f;
            model.SetVelocity(velocity);
            HandleGroundPhysics(delta);
        }
        else HandleAirPhysics(delta);

        if (!SnapUpStairsCheck(delta)) {
            PushAwayRigidBodies();
            model.MoveAndSlide();
            SnapToStairsCheck();
        }
        
        _inputThisFrame = false;

        SlideCamToOrigin(delta);

        _intendedDirection = _inertia <= 0.0f ? Vector3.Zero : _intendedDirection;
        _lastDirection = _leanInertia <= 0.0f ? Vector3.Zero : _lastDirection;
        _jumping = false;
    }
    
    /* --- ---  LISTENERS  --- --- */

    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnCrouchEvent(ActorCrouchEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor())) return;
        if (_lockedMode) {
            ev.SetCanceled(true);
            return;
        }
        
        if (!_crouching || ev.IsStartCrouch()) return;
        if (CanUncrouch()) return;
        ev.SetCanceled(true);
        _tryUncrouch = true;
    }

    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnPickUpItem(ActorPickUpEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor())) return;
        if (_lockedMode) ev.SetCanceled(true);
    }

    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnPlayerUseClick(PlayerUseClickEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor())) return;
        if (_lockedMode) ev.SetCanceled(true);
    }

    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnPlayerJump(PlayerJumpEvent ev, ActorBase player) {
        if (!player.Equals(GetActor())) return;
        if (_lockedMode) ev.SetCanceled(true);
    }

    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnPlayerMove(PlayerMoveEvent ev, Player player) {
        if (!player.Equals(GetActor())) return;
        if (_lockedMode) ev.SetCanceled(true);
    }


    /* --- ---  MOVEMENT  --- --- */
    
    protected bool IsSurfaceTooSteep(Vector3 normal) => normal.AngleTo(Vector3.Up) > GetActor().GetModel().FloorMaxAngle;
    
    protected bool CanUncrouch() => !GetActor().GetModel().TestMove(GetActor().GetModel().Transform, new Vector3(0.0f, CROUCH_TRANSLATE, 0.0f));

    private void SlideCamToOrigin(float delta) {
        if (GetActor() is not IViewable actor) return;

        Camera3D camera = actor.GetCamera();
        Node3D camTarget = actor.GetCamContainer();

        Vector3 targetGlobal = camTarget.GlobalPosition;
        Vector3 currentGlobal = _lastCamGlobalPosition ?? camera.GlobalPosition;

        currentGlobal.X = targetGlobal.X;
        currentGlobal.Z = targetGlobal.Z;

        float moveAmount = Math.Max(0.001f, targetGlobal.DistanceSquaredTo(currentGlobal) * (_snappedToStairs ? 10.0f : 40.0f));
        currentGlobal.Y = Mathf.MoveToward(currentGlobal.Y, targetGlobal.Y, moveAmount * delta * 8.0f);

        camera.GlobalPosition = currentGlobal;
        _lastCamGlobalPosition = currentGlobal;
    }
    
    protected void HandleGroundPhysics(float delta) {
        CharacterBody3D model = GetActor().GetModel();

        _inertia = Math.Clamp(_inertia + (_inputThisFrame ? INERTIA_GAIN : -INERTIA_LOSS), 0.0f, 1.0f);
        
        float speed = GetSpeed() * _inertia;
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

        velocity.Y -= GameManager.GRAVITY * delta;
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
    
    protected bool RunBodyTestMotion(Transform3D from, Vector3 motion, PhysicsTestMotionResult3D result = null) {
        result ??= new PhysicsTestMotionResult3D();
        PhysicsTestMotionParameters3D physicsParams = new();

        physicsParams.From = from;
        physicsParams.Motion = motion;

        return PhysicsServer3D.BodyTestMotion(GetActor().GetModel().GetRid(), physicsParams, result);
    }

    private bool SnapUpStairsCheck(float delta) {
        CharacterBody3D model = GetActor().GetModel();
        if ((!model.IsOnFloor() && !_snappedToStairs) || _jumping)
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

                float velocityDiff = model.GetVelocity().Dot(pushDirection) - rigidBody.LinearVelocity.Dot(pushDirection);
                velocityDiff = Math.Max(0.0f, velocityDiff);

                if (velocityDiff < MIN_IMPULSE_THRESHOLD) continue;
                
                pushDirection.Y = 0.0f;

                float pushForce = massRatio * 2.0f; // Magic adjustment factor

                Vector3 impulse = VectorUtils.RoundTo(pushDirection * velocityDiff * pushForce, 4);
                Vector3 position = collision.GetPosition() - rigidBody.GetGlobalPosition();

                rigidBody.ApplyImpulse(impulse, position);
            }
        }
    }

    protected void HandleCrouch(float delta) {
        if (GetActor() is not Player player) return;
        if (_actorHeight < 0.0f) return;
        
        if (_tryUncrouch && CanUncrouch()) {
            _crouching = false;
            _tryUncrouch = false;
        }

        CharacterBody3D model = player.GetModel();

        float transY = 0.0f;
        if (_crouchingLastFrame != _crouching && !model.IsOnFloor() && !_snappedToStairs)
            transY = _crouching ? CROUCH_JUMP_HEIGHT : -CROUCH_JUMP_HEIGHT;

        Node3D crouchNode = player.GetCrouchNode();
        
        if (transY != 0.0f) {
            KinematicCollision3D result = new();
            model.TestMove(model.Transform, new Vector3(0.0f, transY, 0.0f), result);
            Vector3 modelPos = model.GetPosition();
            Vector3 headPos = crouchNode.GetPosition();
            modelPos.Y += result.GetTravel().Y;
            headPos.Y -= result.GetTravel().Y;
            headPos.Y = Mathf.Clamp(headPos.Y, -CROUCH_TRANSLATE, 0.0f);
            model.SetPosition(modelPos);
            crouchNode.SetPosition(headPos);
        }
        
        Vector3 towards = new(0.0f, _crouching ? -CROUCH_TRANSLATE : 0.0f, 0.0f);
        towards.Y = Mathf.MoveToward(crouchNode.GetPosition().Y, towards.Y, 7.0f * delta);
        crouchNode.SetPosition(towards);

        CollisionShape3D collisionShape = player.GetCollisionShape();
        CapsuleShape3D capsule = (CapsuleShape3D) collisionShape.GetShape();
        float actorHeight = _crouching ? _actorHeight - CROUCH_TRANSLATE : _actorHeight;
        capsule.Height = actorHeight;
        Vector3 position = collisionShape.GetPosition();
        position.Y = actorHeight / 2;
        collisionShape.SetPosition(position);
        
        _crouchingLastFrame = _crouching;
    }
}