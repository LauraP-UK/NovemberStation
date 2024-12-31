using System;
using Godot;

public class PlayerController : ControllerBase {
    private const float SPEED = 5.0f, HOLD_DISTANCE = 2.1f, HOLD_SMOOTHNESS = 15.0f, ROTATION_SMOOTHNESS = 10.0f;
    private const long JUMP_COOLDOWN_MILLIS = 250L;
    private long _lastJump;

    private RigidBody3D _heldObject;
    private Direction _heldObjectDirection;

    public PlayerController(Player player) : base(player) { }

    [EventListener(PriorityLevels.HIGHEST)]
    private void OnPickUpItem(ActorPickUpEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor())) return;
        RigidBody3D hitBody = ev.GetItem();
        
        if (hitBody == null || hitBody.Equals(_heldObject) || _heldObject != null) {
            if (_heldObject != null) {
                _heldObject.FreezeMode = RigidBody3D.FreezeModeEnum.Static;
                _heldObject.Freeze = false;
                
                Vector3 tossDirection = -((Player)GetActor()).GetCamera().GlobalTransform.Basis.Z * 0.1f;
                Vector3 gravityAssist = Vector3.Down * 1.0f;
                _heldObject.LinearVelocity += tossDirection + gravityAssist;
                
                _heldObject = null;
                _heldObjectDirection = null;
            }

            ev.SetCanceled(true);
            return;
        }

        _heldObject = hitBody;
        _heldObjectDirection = FindClosestFace(_heldObject);
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        CharacterBody3D model = player.GetModel();

        if (!direction.Equals(Vector3.Zero)) {
            Vector3 forward = model.GlobalTransform.Basis.Z.Normalized();
            Vector3 right = model.GlobalTransform.Basis.X.Normalized();
            Vector3 velocity = (forward * direction.Z + right * direction.X) * SPEED;

            if (direction.Y > 0.0f && model.IsOnFloor() && TimeSinceLastJump() >= JUMP_COOLDOWN_MILLIS) {
                velocity.Y = direction.Y * 4.5f;
                _lastJump = GetCurrentTimeMillis();
            }

            _velocityInfluence = velocity;
        }

        Vector2 turnDelta = ev.GetTurnDelta();
        if (!turnDelta.Equals(Vector2.Zero)) {
            Camera3D camera3D = player.GetCamera();

            Vector3 modelRotation = model.Rotation;
            Vector3 cameraRotation = camera3D.RotationDegrees;

            float newPitch = Mathf.Clamp(cameraRotation.X - turnDelta.Y * 0.5f, -90.0f, 90.0f);

            model.Rotation = new Vector3(modelRotation.X, modelRotation.Y - turnDelta.X * 0.01f, modelRotation.Z);
            camera3D.RotationDegrees = new Vector3(newPitch, cameraRotation.Y, cameraRotation.Z);
        }
    }

    private static long GetCurrentTimeMillis() => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    private long TimeSinceLastJump() => GetCurrentTimeMillis() - _lastJump;

    protected override void OnUpdate(float delta) {
        if (_heldObject != null) UpdateHeldObjectPosition(delta);
    }

    private void UpdateHeldObjectPosition(double delta) {
        Camera3D camera = ((Player)GetActor()).GetCamera();

        Vector3 targetPosition = camera.GlobalTransform.Origin + -camera.GlobalTransform.Basis.Z * HOLD_DISTANCE;
        Vector3 currentPosition = _heldObject.GlobalTransform.Origin;

        Vector3 directionToTarget = (targetPosition - currentPosition).Normalized();
        float distanceToTarget = currentPosition.DistanceTo(targetPosition);
        Vector3 targetVelocity = directionToTarget * (HOLD_SMOOTHNESS * distanceToTarget);
        
        _heldObject.LinearVelocity = targetVelocity;
        _heldObject.AngularVelocity = Vector3.Zero;
        
        RotateFaceToPlayer(_heldObject, (float)delta);
    }

    public Direction FindClosestFace(RigidBody3D obj) {
        Transform3D globalTransform = obj.GlobalTransform;
        Basis globalBasis = globalTransform.Basis;
        Vector3 playerLookVector = -((Player)GetActor()).GetCamera().GlobalTransform.Basis.Z.Normalized();
        
        Direction closestDirection = null;
        float maxDot = float.NegativeInfinity;

        foreach (Direction direction in Directions.GetAdjacent()) {
            Vector3 globalNormal = globalBasis * direction.Offset;
            float dot = globalNormal.Dot(playerLookVector);

            if (dot > maxDot) {
                maxDot = dot;
                closestDirection = direction;
            }
        }

        return closestDirection;
    }

    private void RotateFaceToPlayer(RigidBody3D obj, float delta) {
        Vector3 targetRotation;
        Vector3 playerRotation = ((Player)GetActor()).GetCamera().GlobalRotation;
        float playerYaw = playerRotation.Y;

        Quaternion currentRotation = obj.GlobalTransform.Basis.GetRotationQuaternion();

        if (_heldObjectDirection.SimpleDirection is SimpleDirection.UP or SimpleDirection.DOWN) {
            float pitchOffset = _heldObjectDirection.SimpleDirection == SimpleDirection.DOWN ? 0.0f : Mathf.DegToRad(180.0f);
            float targetPitch = playerYaw + pitchOffset;

            targetRotation = new Vector3(Mathf.DegToRad(90.0f), targetPitch, 0.0f);
        }
        else {
            float yawOffset = 0.0f;
            if (_heldObjectDirection.Offset == Vector3.Forward)
                yawOffset = 0.0f;
            else if (_heldObjectDirection.Offset == Vector3.Right)
                yawOffset = Mathf.DegToRad(90.0f);
            else if (_heldObjectDirection.Offset == Vector3.Back)
                yawOffset = Mathf.DegToRad(180.0f);
            else if (_heldObjectDirection.Offset == Vector3.Left)
                yawOffset = Mathf.DegToRad(-90.0f);

            float targetYaw = playerYaw + yawOffset;
            targetRotation = new Vector3(0.0f, targetYaw, 0.0f);
        }
        Quaternion smoothedRotation = currentRotation.Slerp(Quaternion.FromEuler(targetRotation), ROTATION_SMOOTHNESS * delta);
        
        Basis smoothedBasis = new(smoothedRotation);
        obj.GlobalTransform = new Transform3D(smoothedBasis, obj.GlobalTransform.Origin);
    }
}