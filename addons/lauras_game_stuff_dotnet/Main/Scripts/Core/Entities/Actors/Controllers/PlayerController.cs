
using Godot;

public class PlayerController : ControllerBase {

    private const float SPEED = 5.0f;
    
    public PlayerController(Player player) : base(player) {}

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        CharacterBody3D model = player.GetModel();

        if (!direction.Equals(Vector3.Zero)) {
            Vector3 forward = model.GlobalTransform.Basis.Z.Normalized();
            Vector3 right = model.GlobalTransform.Basis.X.Normalized();
            Vector3 velocity = (forward * direction.Z + right * direction.X) * SPEED;
            _velocityInfluence = new Vector3(velocity.X, direction.Y * 4.5f, velocity.Z);
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