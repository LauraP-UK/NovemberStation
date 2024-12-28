
using System;
using Godot;

public class PlayerController : ControllerBase {

    private const float SPEED = 5.0f, HOLD_DISTANCE = 2.5f, HOLD_SMOOTHNESS = 10.0f;
    private const long JUMP_COOLDOWN_MILLIS = 250L;
    private long _lastJump;

    private RigidBody3D _heldObject;
    
    public PlayerController(Player player) : base(player) {}

    [EventListener(PriorityLevels.HIGHEST)]
    private void OnPickUpItem(ActorPickUpEvent ev, ActorBase actor) {
        if (!actor.Equals(GetActor())) return;
        RigidBody3D hitBody = ev.GetItem();
        if (hitBody == null || hitBody.Equals(_heldObject)) {
            _heldObject = null;
            ev.SetCanceled(true);
            return;
        }
        
        _heldObject = hitBody;
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
    
    private long GetCurrentTimeMillis() => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    private long TimeSinceLastJump() => GetCurrentTimeMillis() - _lastJump;
    protected override void OnUpdate(float delta) {
        if (_heldObject != null) UpdateHeldObjectPosition(delta);
    }
    
    private void UpdateHeldObjectPosition(double delta) {
        Camera3D camera = ((Player)GetActor()).GetCamera();

        // Calculate target position and direction
        Vector3 targetPosition = camera.GlobalTransform.Origin + -camera.GlobalTransform.Basis.Z * HOLD_DISTANCE;
        Vector3 currentPosition = _heldObject.GlobalTransform.Origin;

        // Calculate the velocity needed to move towards the target
        Vector3 directionToTarget = (targetPosition - currentPosition).Normalized();
        float distanceToTarget = currentPosition.DistanceTo(targetPosition);
        Vector3 targetVelocity = directionToTarget * (HOLD_SMOOTHNESS * distanceToTarget);

        Vector3 rotation = camera.GlobalRotation;
        rotation.X = 0; // Prevent pitch/roll of object

        _heldObject.Rotation = rotation;
        
        _heldObject.LinearVelocity = targetVelocity;
        
        _heldObject.AngularVelocity = Vector3.Zero;
    }
}