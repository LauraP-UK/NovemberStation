
using Godot;

public class PlayerController : ControllerBase {
    
    public PlayerController(Player player) : base(player) {}

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerJump(PlayerJumpEvent ev, Player player) {
        CharacterBody3D model = player.GetModel();
        if (!model.IsOnFloor()) return;
        //GD.Print("INFO: PlayerController.OnPlayerJump() : Player is not on the floor and has Jumped!");
        _velocityInfluence = new Vector3(0.0f, 5.5f, 0.0f);
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        CharacterBody3D model = player.GetModel();

        if (!direction.Equals(Vector3.Zero)) {
            float speed = 5.0f;
            
            //GD.Print($"Forward: {model.GlobalTransform.Basis.Z}, Right: {model.GlobalTransform.Basis.X}");

            Vector3 forward = model.GlobalTransform.Basis.Z.Normalized();
            Vector3 right = model.GlobalTransform.Basis.X.Normalized();
            Vector3 velocity = (forward * direction.Z + right * direction.X) * speed;
            _velocityInfluence = new Vector3(velocity.X, 0, velocity.Z);

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