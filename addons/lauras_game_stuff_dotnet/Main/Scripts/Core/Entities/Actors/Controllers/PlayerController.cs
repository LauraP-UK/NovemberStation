
using Godot;

public class PlayerController : ControllerBase {

    private const int MOVE_RANGE = 5;
    
    public PlayerController(Player player) : base(player) {}

    /*[EventListener(PriorityLevels.NORMAL)]
    private void OnPlayerMoveCatcher(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        Vector3 newPosition = GetPredictedMovement(direction * 0.1f);
        if (!direction.Equals(Vector3.Zero))
            if (newPosition.X < -MOVE_RANGE || newPosition.X > MOVE_RANGE || newPosition.Z < -MOVE_RANGE || newPosition.Z > MOVE_RANGE)
                ev.SetCanceled(true);
    }*/

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        CharacterBody3D model = player.GetModel();

        if (!direction.Equals(Vector3.Zero)) {
            float speed = 5f;
            Vector3 forward = model.GlobalTransform.Basis.Z;
            Vector3 right = model.GlobalTransform.Basis.X;
            Vector3 targetVelocity = (forward * direction.Z + right * direction.X) * speed;
            model.Velocity = targetVelocity;
        }

        Vector2 turnDelta = ev.GetTurnDelta();
        if (!turnDelta.Equals(Vector2.Zero)) {
            Camera3D camera3D = player.GetCamera();

            Vector3 modelRotation = model.Rotation;
            Vector3 cameraRotation = camera3D.RotationDegrees;
            
            float newPitch = Mathf.Clamp(cameraRotation.X - turnDelta.Y * 0.5f, -89.9f, 89.9f);
            
            model.Rotation = new Vector3(modelRotation.X, modelRotation.Y - turnDelta.X * 0.01f, modelRotation.Z);
            camera3D.RotationDegrees = new Vector3(newPitch, cameraRotation.Y, cameraRotation.Z);
        }
    }
}