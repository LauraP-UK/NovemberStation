using System;
using Godot;

public class PlayerController : ControllerBase<Player> {
    private const float
        HOLD_DISTANCE = 2.1f,
        HOLD_DISTANCE_MIN = 0.5f,
        HOLD_DISTANCE_MAX = 1.5f,
        HOLD_SMOOTHNESS = 15.0f,
        ROTATION_SMOOTHNESS = 10.0f;
    private const long
        JUMP_COOLDOWN_MILLIS = 250L;
    private const uint
        PLAYER_LAYER = 1 << 0, // Layer 1
        STATIC_LAYER = 1 << 1, // Layer 2
        OBJECT_LAYER = 1 << 2; // Layer 3
    
    private static readonly Vector3
        FORWARD_LEAN_ROTATION = new(-7.5f, 0.0f, 0.0f),
        BACKWARD_LEAN_ROTATION = new(2.0f, 0.0f, 0.0f),
        LEFT_LEAN_ROTATION = new(0.0f, 0.0f, 4.5f),
        RIGHT_LEAN_ROTATION = new(0.0f, 0.0f, -4.5f);

    private bool _asleep, _altAction, _toggleCrouch, _uiVisible = true, _debugObjects = false;
    private float _holdDistanceModifier = 1.0f;
    private Vector2 _rotationOffset = Vector2.Zero, _targetLook, _currentLook;
    private long _lastJump;
    private int _actionIndex;
    private ulong _lastActionID;
    private string _lastBehaviourType = "";
    private Node3D _contextObject;

    private RigidBody3D _heldObject;
    private Direction _heldObjectDirection;

    private readonly ContextMenu _contextMenu = new();
    private readonly CrosshairOverlay _crosshair = new();
    private readonly ShopMenu _shopMenu = new();
    private readonly ToastUI _toastUI = new();
    private readonly HotbarMenu _hotbarMenu = new();

    public PlayerController(Player player) : base(player) {
        player.GetModel().CollisionLayer = PLAYER_LAYER;
        _contextMenu.Open();
        _crosshair.Open();
        _toastUI.Open();
        _hotbarMenu.Open();
        _hotbarMenu.GetForm().SetOwner(player);
    }


    /* --- ---  LISTENERS  --- --- */
    [EventListener]
    private void ToggleUI(KeyPressEvent ev, Key key) {
        if (IsLocked()) return;
        if (key != Key.F1) return;
        ShowUI(!_uiVisible);
    }

    [EventListener]
    private void ToggleDebugObjects(KeyPressEvent ev, Key key) {
        if (IsLocked()) return;
        if (key != Key.F2 || !GameManager.IsDebugMode()) return;
        _debugObjects = !_debugObjects;
        GameManager.I().DebugObjects(_debugObjects);
    }

    [EventListener]
    private void OnEscape(KeyPressEvent ev, Key key) {
        if (key != Key.Escape || !IsAsleep()) return;
        ev.Capture();
        WakeUp();
    }

    [EventListener]
    private void OnInventoryOpen(KeyPressEvent ev, Key key) {
        if (IsLocked() || ev.IsCaptured() || key != Key.Tab) return;
        ev.Capture();
        new SingleInvDisplayMenu().Open();
    }

    [EventListener]
    private void OnOpenShop(KeyPressEvent ev, Key key) {
        if (IsLocked()) return;
        if (key != Key.V) return;
        _shopMenu.Open();
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
        bool isDown = button == MouseButton.WheelDown;

        if (!IsHoldingObject() && GetContextObject() == null) {
            Player player = GetActor();
            Hotbar hotbar = player.GetHotbar();

            hotbar.ChangeIndex(isDown ? 1 : -1);
            player.GetHotbar().ResyncInventory();
            player.GetHotbar().UpdateOwnerHeldItem();
        }

        if (IsHoldingObject()) {
            _holdDistanceModifier = Mathf.Clamp(_holdDistanceModifier + (isDown ? -0.05f : 0.05f), HOLD_DISTANCE_MIN, HOLD_DISTANCE_MAX);
            return;
        }

        if (GetContextObject() != null) _actionIndex += isDown ? 1 : -1;
    }

    [EventListener]
    private void OnPickUpItem(ActorPickUpEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor())) return;
        RigidBody3D hitBody = ev.GetItem();
        if (hitBody == null || hitBody.Equals(_heldObject) || _heldObject != null) {
            ReleaseHeldObject(Vector3.Down * 0.2f); // Mini nudge down
            return;
        }

        PickupObject(hitBody);
    }

    [EventListener]
    private void OnMouseClick(MouseInputEvent ev, Vector2 position) {
        if (ev.GetMouseButton() != MouseButton.Left || !ev.IsPressed() || ev.IsCaptured()) return;
        if (_heldObject != null) return;
        IObjectBase handItem = GetActor().GetHandItem();
        switch (handItem) {
            case IUsable usable when handItem.TestAction<IUsable>(GetActor(), ev): usable.Use(GetActor(), ev); break;
            case IDrinkable drinkable when handItem.TestAction<IDrinkable>(GetActor(), ev): drinkable.Drink(GetActor(), ev); break;
        }

        GetActor().GetHotbar().ResyncInventory();
    }

    [EventListener]
    private void OnMouseMiddleClick(MouseInputEvent ev, Vector2 position) {
        if (ev.GetMouseButton() != MouseButton.Middle || !ev.IsPressed() || ev.IsCaptured()) return;
        if (_heldObject != null) return;
        Player owner = GetActor();
        IObjectBase objectBase = owner.GetHandItem();
        if (objectBase == null) return;

        string toDropJson = objectBase.Serialise();
        ObjectAtlas.CreatedObject obj = ObjectAtlas.CreatedObjectFromJson(toDropJson);
        if (!obj.Success) return;

        RaycastResult result = owner.GetLookingAt(2);

        Vector3 spawn =
            result.HasHit() ? result.GetClosestHit().HitAtPosition + result.GetClosestHit().HitNormal * 0.2f : result.GetEnd();

        Node3D objNode = (Node3D)obj.Node;
        GameManager.I().GetSceneObjects().AddChild(obj.Node);
        objNode.SetGlobalPosition(spawn);
        objNode.SetGlobalRotation(new Vector3(0, owner.GetCamera().GlobalRotation.Y, 0));
        GameManager.I().RegisterObject(objNode, obj.Object);

        owner.GetHotbar().ResyncInventory();

        owner.ClearHeldItem();
        owner.RemoveItem(toDropJson);
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
        if (!direction.Equals(Vector3.Zero)) {
            _intendedDirection = GetActor().GetModel().GlobalTransform.Basis * direction;
            _lastDirection = direction;
            _inputThisFrame = true;
        }

        Vector2 turnDelta = ev.GetTurnDelta();
        if (turnDelta.Equals(Vector2.Zero)) return;
        if (_altAction && _heldObject != null) _rotationOffset += turnDelta * 0.02f;
        else _targetLook += turnDelta * 0.5f;
    }

    /* --- ---  METHODS  --- --- */

    private static long GetCurrentTimeMillis() => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    private long TimeSinceLastJump() => GetCurrentTimeMillis() - _lastJump;
    private bool IsHoldingObject() => _heldObject != null;

    protected override void OnUpdate(float delta) {
        Player player = GetActor();

        if (!IsAsleep()) {
            Vector3 targetLean = GetAmalgamatedLean(_lastDirection) * _leanInertia;
            _leanTarget = _leanTarget.Lerp(targetLean, 10.0f * delta);
            player.GetLeanNode().RotationDegrees = _leanTarget;
            HandleLookRot(delta);
        }
        HandleContextMenu();
        player.GetHotbar().ResyncInventory();
    }

    protected override void OnPhysicsUpdate(float delta) {
        if (_heldObject != null) UpdateHeldObjectPosition(delta);
    }

    public void Sleep() {
        _asleep = true;
        SetLocked(true);
        ShowUI(false);
        GameManager.SetEngineSpeed(30.0f);
        GameManager.I().GetSleepCamera().MakeCurrent();
    }

    public void WakeUp() {
        _asleep = false;
        SetLocked(false);
        ShowUI(true);
        GameManager.ResetEngineSpeed();
        GetActor().AssumeCameraControl();
    }

    public ActionKey? GetCurrentContextAction() {
        ActionDisplayButton btn = _contextMenu.GetForm().GetAction(_actionIndex);
        return btn?.GetAction();
    }

    public Node3D GetContextObject() => _heldObject ?? _contextObject;
    public bool IsAsleep() => _asleep;

    private Vector3 GetAmalgamatedLean(Vector3 dir) {
        Vector3 toReturn = Vector3.Zero;
        float leanFactor = _sprinting ? 1.0f : 0.25f;
        toReturn.Z = dir.X switch {
            > 0.0f => Mathf.Lerp(0, RIGHT_LEAN_ROTATION.Z * leanFactor, dir.X),
            < 0.0f => Mathf.Lerp(0, LEFT_LEAN_ROTATION.Z * leanFactor, -dir.X),
            _ => toReturn.Z
        };
        
        toReturn.X = dir.Z switch {
            > 0.0f => Mathf.Lerp(0, BACKWARD_LEAN_ROTATION.X * leanFactor, dir.Z),
            < 0.0f => Mathf.Lerp(0, FORWARD_LEAN_ROTATION.X * leanFactor, -dir.Z),
            _ => toReturn.X
        };

        return toReturn;
    }

    private void ReleaseHeldObject(Vector3? releaseVelocity = null) {
        if (_heldObject == null) return;

        _heldObject.CollisionMask |= PLAYER_LAYER;
        _heldObject.CollisionLayer |= OBJECT_LAYER;

        _heldObject.LinearVelocity += releaseVelocity ?? Vector3.Down;

        _heldObject = null;
        _heldObjectDirection = null;
    }

    private void PickupObject(RigidBody3D obj) {
        _holdDistanceModifier = 1.0f;
        _rotationOffset = Vector2.Zero;

        RaycastResult raycast = Raycast.Trace(GetActor(), obj.GlobalPosition);
        if (raycast.HasHit()) {
            _holdDistanceModifier = Mathsf.Remap(
                HOLD_DISTANCE * HOLD_DISTANCE_MIN,
                HOLD_DISTANCE * HOLD_DISTANCE_MAX,
                obj.GlobalPosition.DistanceTo(raycast.GetStart()),
                HOLD_DISTANCE_MIN,
                HOLD_DISTANCE_MAX
            );
            _holdDistanceModifier = Mathsf.RoundTo(_holdDistanceModifier, 0.05f);
        }

        _heldObject = obj;
        _heldObject.CollisionMask &= ~PLAYER_LAYER;
        _heldObject.CollisionLayer &= ~OBJECT_LAYER;
        _heldObjectDirection = FindClosestFace(_heldObject);
    }

    private void UpdateHeldObjectPosition(double delta) {
        Camera3D camera = GetActor().GetCamera();

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
        Vector3 playerLookVector = -GetActor().GetCamera().GlobalTransform.Basis.Z.Normalized();

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
        Vector3 playerRotation = GetActor().GetCamera().GlobalRotation;
        float playerYaw = playerRotation.Y;

        Quaternion currentRotation = obj.GlobalTransform.Basis.GetRotationQuaternion();

        if (_heldObjectDirection.SimpleDirection is SimpleDirection.UP or SimpleDirection.DOWN) {
            float pitchOffset = _heldObjectDirection.SimpleDirection == SimpleDirection.DOWN ? 0.0f : Mathf.DegToRad(180.0f);
            float targetPitch = playerYaw + pitchOffset;

            targetRotation = new Vector3(Mathf.DegToRad(90.0f), targetPitch, 0.0f);
        } else {
            float yawOffset = 0.0f;
            Vector3 offset = _heldObjectDirection.Offset;
            if (offset.Equals(Vector3.Forward))
                yawOffset = 0.0f;
            else if (offset.Equals(Vector3.Right))
                yawOffset = Mathf.DegToRad(90.0f);
            else if (offset.Equals(Vector3.Back))
                yawOffset = Mathf.DegToRad(180.0f);
            else if (offset.Equals(Vector3.Left)) yawOffset = Mathf.DegToRad(-90.0f);

            float targetYaw = playerYaw + yawOffset;
            targetRotation = new Vector3(0.0f, targetYaw, 0.0f);
        }

        targetRotation.Y += _rotationOffset.X;
        targetRotation.X += -_rotationOffset.Y;

        Quaternion smoothedRotation = currentRotation.Slerp(Quaternion.FromEuler(targetRotation), ROTATION_SMOOTHNESS * delta);

        Basis smoothedBasis = new(smoothedRotation);
        obj.GlobalTransform = new Transform3D(smoothedBasis, obj.GlobalTransform.Origin);
    }

    private void HandleLookRot(float delta) {
        Player player = GetActor();
        float smoothness = player.GetLookSmoothness();
        _currentLook = _currentLook.MoveToward(_targetLook, smoothness * delta);
        
        float yawDelta = _currentLook.X;
        float pitchDelta = _currentLook.Y;
        
        Camera3D camera3D = player.GetCamera();
        CharacterBody3D model = player.GetModel();
        
        Vector3 modelRotation = model.Rotation;
        Vector3 cameraRotation = camera3D.RotationDegrees;
        
        float newPitch = Mathf.Clamp(cameraRotation.X - pitchDelta * 0.5f, -90.0f, 90.0f);
        model.Rotation = new Vector3(modelRotation.X, modelRotation.Y - yawDelta * 0.015f, modelRotation.Z);
        camera3D.RotationDegrees = new Vector3(newPitch, cameraRotation.Y, cameraRotation.Z);

        _targetLook = Vector2.Zero;
    }

    /* --- ---  UI  --- --- */
    private void HandleContextMenu() {
        Camera3D activeCamera = GameManager.I().GetActiveCamera();
        RaycastResult raycastResult = _heldObject != null ? Raycast.TraceActive(_heldObject) : Raycast.TraceActive(3.0f);
        RaycastResult.HitBodyData contextObjResult = _heldObject != null ? raycastResult.GetViaBody(_heldObject) : raycastResult.GetClosestHit();

        if (contextObjResult == null && _heldObject == null) {
            _contextObject = null;
            HideContextBox();
            return;
        }

        _contextObject = _heldObject ?? contextObjResult.Body;
        Node sceneRoot = GameUtils.FindSceneRoot(_contextObject);
        if (sceneRoot == null) {
            _contextObject = null;
            HideContextBox();
            return;
        }

        ulong instanceId = sceneRoot.GetInstanceId();

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
            if (_lastBehaviourType != objectData.GetObjectTag()) {
                _actionIndex = 0;
                _lastBehaviourType = objectData.GetObjectTag();
            }
        }

        (Vector2 minPos, Vector2 maxPos) = Projections.Project(shape, activeCamera, shape.GlobalTransform);

        float distanceTo = !_uiVisible ? 10f : contextObjResult?.Distance ?? _contextObject.GlobalPosition.DistanceTo(activeCamera.GlobalPosition);
        float distRatio = Mathsf.InverseLerpClamped(2.9f, 0.9f, distanceTo);
        float titleRatio = Mathsf.InverseLerpClamped(2.9f, 2.5f, distanceTo);

        ContextMenuForm form = _contextMenu.GetForm();
        form?.Draw(_actionIndex, minPos, maxPos, distRatio, titleRatio, _heldObject == null ? titleRatio : 0.0f, objectData);
    }

    private void HideContextBox() {
        _lastActionID = 0;
        if (_contextMenu.GetForm() == null) return;
        _contextMenu.GetForm().Hide();
    }

    public ToastUI GetToastUI() => _toastUI;
    public HotbarMenu GetHotbarMenu() => _hotbarMenu;

    public void ShowUI(bool show) {
        _uiVisible = show;
        _contextMenu.GetForm().GetMenu().SetVisible(show);
        _crosshair.GetForm().GetMenu().SetVisible(show);
        _hotbarMenu.GetForm().GetMenu().SetVisible(show);
    }
}