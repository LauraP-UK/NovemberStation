using System;
using Godot;

public class PlayerController : ControllerBase {
    private const float HOLD_DISTANCE = 2.1f, HOLD_SMOOTHNESS = 15.0f, ROTATION_SMOOTHNESS = 10.0f;
    private const long JUMP_COOLDOWN_MILLIS = 250L;

    private const uint PLAYER_LAYER = 1 << 0, // Layer 1
        STATIC_LAYER = 1 << 1, // Layer 2
        OBJECT_LAYER = 1 << 2; // Layer 3

    private bool _altAction, _toggleCrouch;
    private float _holdDistanceModifier = 1.0f;
    private Vector2 _rotationOffset = Vector2.Zero;
    private long _lastJump;
    private int _actionIndex;
    private ulong _lastActionID;
    private string _lastBehaviourType = "";
    private Node3D _contextObject;

    private RigidBody3D _heldObject;
    private Direction _heldObjectDirection;

    private readonly ContextMenu _contextMenu = new();
    private readonly CrosshairOverlay _crosshair = new();

    public PlayerController(Player player) : base(player) {
        GetActor().GetModel().CollisionLayer = PLAYER_LAYER;
        _contextMenu.Open();
        _crosshair.Open();
    }


    /* --- ---  LISTENERS  --- --- */
    [EventListener]
    private void OnOpenShop(KeyPressEvent ev, Key key) {
        if (IsLocked()) return;
        if (key != Key.V) return;

        new ShopMenu().Open();
    }

    [EventListener]
    private void OnCrouchToggle(KeyPressEvent ev, Key key) {
        if (IsLocked()) return;
        if (key != Key.C) return;
        _toggleCrouch = !_toggleCrouch;
    }

    [EventListener]
    private void OnAltHeld(MouseInputEvent ev, Vector2 position) {
        if (IsLocked()) return;
        if (_altAction || !(ev.GetMouseButton() == MouseButton.Right && ev.IsPressed())) return;
        _altAction = true;
    }

    [EventListener]
    private void OnAltReleased(MouseInputEvent ev, Vector2 position) {
        if (!(ev.GetMouseButton() == MouseButton.Right && !ev.IsPressed())) return;
        _altAction = false;
    }

    [EventListener]
    private void OnShiftHeld(KeyPressEvent ev, Key key) {
        if (IsLocked()) return;
        if (key != Key.Shift || _sprinting) return;
        _sprinting = true;
    }

    [EventListener]
    private void OnShiftReleased(KeyReleaseEvent ev, Key key) {
        if (key != Key.Shift) return;
        _sprinting = false;
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnCrouchEvent(ActorCrouchEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor())) return;
        if (_toggleCrouch) {
            if (!ev.IsStartCrouch()) return;
            if (_crouching && !CanUncrouch()) _tryUncrouch = true;
            else _crouching = !_crouching;
            return;
        }

        _crouching = ev.IsStartCrouch();
    }

    [EventListener]
    private void OnMouseScroll(MouseInputEvent ev, Vector2 position) {
        if (IsLocked() || !ev.IsPressed()) return;
        MouseButton button = ev.GetMouseButton();
        if (button != MouseButton.WheelDown && button != MouseButton.WheelUp) return;
        
        if (IsHoldingObject()) {
            _holdDistanceModifier = Mathf.Clamp(_holdDistanceModifier + (button == MouseButton.WheelDown ? -0.05f : 0.05f), 0.5f, 1.5f);
            return;
        }
        if (GetContextObject() != null) _actionIndex += button == MouseButton.WheelDown ? 1 : -1;
    }

    [EventListener]
    private void OnPickUpItem(ActorPickUpEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor())) return;
        RigidBody3D hitBody = ev.GetItem();

        if (hitBody == null || hitBody.Equals(_heldObject) || _heldObject != null) {
            Vector3 tossDirection = -((Player)GetActor()).GetCamera().GlobalTransform.Basis.Z * 0.1f;
            ReleaseHeldObject(tossDirection + Vector3.Down); // Mini push in a direction plus a nudge down
            return;
        }

        PickupObject(hitBody);
    }

    [EventListener]
    private void OnPlayerUseClick(PlayerUseClickEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor()) || !ev.IsPressed() || _heldObject == null) return;
        Vector3 tossDirection = -((Player)GetActor()).GetCamera().GlobalTransform.Basis.Z * 20.0f; // Big push in the direction
        ReleaseHeldObject(tossDirection + Vector3.Down);
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerJump(PlayerJumpEvent ev, ActorBase player) {
        if (!GetActor().GetModel().IsOnFloor() || TimeSinceLastJump() < JUMP_COOLDOWN_MILLIS) return;
        _jumping = true;
        _lastJump = GetCurrentTimeMillis();
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        CharacterBody3D model = player.GetModel();

        if (!direction.Equals(Vector3.Zero))
            _intendedDirection = GetActor().GetModel().GlobalTransform.Basis * direction;

        Vector2 turnDelta = ev.GetTurnDelta();
        if (!turnDelta.Equals(Vector2.Zero)) {
            if (_altAction && _heldObject != null) {
                _rotationOffset += turnDelta * 0.02f;
                return;
            }

            Camera3D camera3D = player.GetCamera();

            Vector3 modelRotation = model.Rotation;
            Vector3 cameraRotation = camera3D.RotationDegrees;

            float newPitch = Mathf.Clamp(cameraRotation.X - turnDelta.Y * 0.5f, -90.0f, 90.0f);

            model.Rotation = new Vector3(modelRotation.X, modelRotation.Y - turnDelta.X * 0.01f, modelRotation.Z);
            camera3D.RotationDegrees = new Vector3(newPitch, cameraRotation.Y, cameraRotation.Z);
        }
    }

    /* --- ---  METHODS  --- --- */

    private static long GetCurrentTimeMillis() => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    private long TimeSinceLastJump() => GetCurrentTimeMillis() - _lastJump;
    private bool IsHoldingObject() => _heldObject != null;
    
    protected override void OnUpdate(float delta) => HandleContextMenu();

    protected override void OnPhysicsUpdate(float delta) {
        if (_heldObject != null) UpdateHeldObjectPosition(delta);
    }

    public Type GetCurrentContextAction() {
        ActionDisplayButton btn = _contextMenu.GetForm().GetAction(_actionIndex);
        return btn?.GetAction();
    }
    public Node3D GetContextObject() => _heldObject ?? _contextObject;

    private void ReleaseHeldObject(Vector3? releaseVelocity = null) {
        if (_heldObject == null) return;

        _heldObject.CollisionMask |= PLAYER_LAYER;
        _heldObject.CollisionLayer |= OBJECT_LAYER;
        _heldObject.FreezeMode = RigidBody3D.FreezeModeEnum.Static;
        _heldObject.Freeze = false;

        _heldObject.LinearVelocity += releaseVelocity ?? Vector3.Down;

        _heldObject = null;
        _heldObjectDirection = null;
    }

    private void PickupObject(RigidBody3D obj) {
        _holdDistanceModifier = 1.0f;
        _rotationOffset = Vector2.Zero;

        _heldObject = obj;
        _heldObject.CollisionMask &= ~PLAYER_LAYER;
        _heldObject.CollisionLayer &= ~OBJECT_LAYER;
        _heldObjectDirection = FindClosestFace(_heldObject);
    }

    private void UpdateHeldObjectPosition(double delta) {
        Camera3D camera = ((Player)GetActor()).GetCamera();

        Vector3 targetPosition = camera.GlobalTransform.Origin + -camera.GlobalTransform.Basis.Z * (HOLD_DISTANCE * _holdDistanceModifier);
        Vector3 currentPosition = _heldObject.GlobalTransform.Origin;

        Vector3 directionToTarget = (targetPosition - currentPosition).Normalized();
        float distanceToTarget = currentPosition.DistanceTo(targetPosition);
        Vector3 targetVelocity = directionToTarget * (HOLD_SMOOTHNESS * distanceToTarget);

        _heldObject.LinearVelocity = targetVelocity;
        _heldObject.AngularVelocity = Vector3.Zero;

        RotateFaceToPlayer(_heldObject, (float)delta);
    }

    private Direction FindClosestFace(RigidBody3D obj) {
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
            Vector3 offset = _heldObjectDirection.Offset;
            if (offset.Equals(Vector3.Forward))
                yawOffset = 0.0f;
            else if (offset.Equals(Vector3.Right))
                yawOffset = Mathf.DegToRad(90.0f);
            else if (offset.Equals(Vector3.Back))
                yawOffset = Mathf.DegToRad(180.0f);
            else if (offset.Equals(Vector3.Left))
                yawOffset = Mathf.DegToRad(-90.0f);

            float targetYaw = playerYaw + yawOffset;
            targetRotation = new Vector3(0.0f, targetYaw, 0.0f);
        }

        targetRotation.Y += _rotationOffset.X;
        targetRotation.X += -_rotationOffset.Y;

        Quaternion smoothedRotation = currentRotation.Slerp(Quaternion.FromEuler(targetRotation), ROTATION_SMOOTHNESS * delta);

        Basis smoothedBasis = new(smoothedRotation);
        obj.GlobalTransform = new Transform3D(smoothedBasis, obj.GlobalTransform.Origin);
    }
    
    /* --- ---  UI  --- --- */
    private void HandleContextMenu() {
        Camera3D activeCamera = GameManager.I().GetActiveCamera();
        RaycastResult raycastResult = Raycast.TraceActive(3.0f);
        RaycastResult.HitBodyData contextObjResult = _heldObject != null ? raycastResult.GetViaBody(_heldObject) : raycastResult.GetClosestHit();

        if (contextObjResult == null && _heldObject == null) {
            _contextObject = null;
            HideContextBox();
            return;
        }

        _contextObject = _heldObject ?? contextObjResult.Body;
        ulong instanceId = GameUtils.FindSceneRoot(_contextObject).GetInstanceId();

        CollisionShape3D shape = (CollisionShape3D)_contextObject.FindChild("BBox");
        if (shape == null) {
            _contextObject = null;
            HideContextBox();
            return;
        }

        IObjectBase objectData = GameManager.I().GetObjectClass<IObjectBase>(instanceId);
        if (objectData != null && instanceId != _lastActionID) {
            HideContextBox();
            _lastActionID = instanceId;
            if (_lastBehaviourType != objectData.GetMetaTag()) {
                _actionIndex = 0;
                _lastBehaviourType =  objectData.GetMetaTag();
            }
        }

        BoundingBox bb = BoundingBox.FromCollisionMesh(shape);
        Vector2[] inScreenSpace = bb.GetCornersInScreenSpace(activeCamera, shape.GlobalTransform);
        VectorUtils.ExtremesInfo2D vecExtremes = VectorUtils.GetExtremes(inScreenSpace);

        Vector2 minPos = vecExtremes.min;
        Vector2 maxPos = vecExtremes.max - minPos;

        float distanceTo = contextObjResult?.Distance ?? _contextObject.GlobalPosition.DistanceTo(activeCamera.GlobalPosition);
        float distRatio = Mathsf.InverseLerpClamped(2.9f, 0.9f, distanceTo);
        float actionRatio = Mathsf.InverseLerpClamped(2.9f, 2.5f, distanceTo);
        
        ContextMenuForm form = _contextMenu.GetForm();
        form?.Draw(_actionIndex, minPos, maxPos, distRatio, actionRatio, objectData);
    }
    
    private void HideContextBox() {
        _lastActionID = 0;
        if (_contextMenu.GetForm() == null) return;
        _contextMenu.GetForm().Hide();
    }
}