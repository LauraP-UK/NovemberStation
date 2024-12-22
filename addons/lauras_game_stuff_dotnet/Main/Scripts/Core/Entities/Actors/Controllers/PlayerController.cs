
using Godot;
using NovemberStation.Main;

public class PlayerController : ControllerBase {

    private const int MOVE_RANGE = 5;
    
    public PlayerController(Player player) : base(player) {}

    [EventListener(PriorityLevels.NORMAL)]
    private void OnPlayerMoveCatcher(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        Vector3 newPosition = player.GetPosition() + direction;
        if (newPosition.X < -MOVE_RANGE || newPosition.X > MOVE_RANGE || newPosition.Z < -MOVE_RANGE || newPosition.Z > MOVE_RANGE) {
            GD.Print("PlayerController:OnPlayerMoveCatcher : Player would move out of bounds!");
            ev.SetCanceled(true);
        }
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(PlayerMoveEvent ev, Player player) {
        Vector3 direction = ev.GetDirection();
        if (!direction.Equals(Vector3.Zero)) player.SetPosition(player.GetPosition() + direction);

        Vector2 turnDelta = ev.GetTurnDelta();
        if (!turnDelta.Equals(Vector2.Zero)) {
            Camera3D camera3D = TestScript.I().GetPlayer().GetCamera();
            CharacterBody3D model = TestScript.I().GetPlayer().GetModel();

            Vector3 modelRotation = model.Rotation;
            model.Rotation = new Vector3(modelRotation.X, modelRotation.Y - turnDelta.X * 0.01f, modelRotation.Z);
            
            Vector3 cameraRotation = camera3D.RotationDegrees;
            float newPitch = Mathf.Clamp(cameraRotation.X - turnDelta.Y * 0.5f, -89.9f, 89.9f);
            camera3D.RotationDegrees = new Vector3(newPitch, cameraRotation.Y, cameraRotation.Z);
        }
    }
}