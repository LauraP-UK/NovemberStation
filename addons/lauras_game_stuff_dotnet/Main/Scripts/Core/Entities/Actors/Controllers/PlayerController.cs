
using Godot;

public class PlayerController : ControllerBase {
    
    public PlayerController(Player player) : base(player) {}

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerJump(PlayerJumpEvent ev, Player player) {
        CharacterBody3D model = player.GetModel();
        if (!model.IsOnFloor()) return;
        GD.Print("INFO: PlayerController.OnPlayerJump() : Player is not on the floor and has Jumped!");
        _velocityInfluence += new Vector3(0.0f, 5.5f, 0.0f);
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        CharacterBody3D model = player.GetModel();

        if (!direction.Equals(Vector3.Zero)) {
            float speed = 5f;
            Vector3 forward = model.GlobalTransform.Basis.Z;
            Vector3 right = model.GlobalTransform.Basis.X;
            
            Vector3 targetVelocity = (forward * direction.Z + right * direction.X);
            
            if (!targetVelocity.Equals(Vector3.Zero)) targetVelocity = targetVelocity.Normalized() * speed;
            
            _velocityInfluence = new Vector3(0.0f, _velocityInfluence.Y, 0.0f);
            _velocityInfluence += targetVelocity;
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
}