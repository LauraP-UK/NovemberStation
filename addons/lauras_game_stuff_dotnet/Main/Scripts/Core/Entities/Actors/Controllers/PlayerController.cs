
using Godot;

public class PlayerController : ControllerBase {

    private const int MOVE_RANGE = 5;
    
    public PlayerController(Player player) : base(player) {}

    [EventListener(PriorityLevels.NORMAL)]
    private void OnPlayerMoveCatcher(ActorMoveEvent ev, Player player) {
        GD.Print("PlayerController:OnPlayerMoveCatcher: Catching player move");
        if (player.GetUUID() != GetActor().GetUUID()) {
            GD.Print("PlayerController:OnPlayerMoveCatcher: Not the right player");
            return;
        }

        Vector3 direction = ev.GetDirection();
        Vector3 newPosition = player.GetPosition() + direction;
        if (newPosition.X < -MOVE_RANGE || newPosition.X > MOVE_RANGE || newPosition.Z < -MOVE_RANGE || newPosition.Z > MOVE_RANGE) {
            GD.Print("PlayerController:OnPlayerMoveCatcher: Player would move out of bounds!");
            ev.SetCanceled(true);
            return;
        }

        GD.Print("PlayerController:OnPlayerMoveCatcher: Event is fine, proceed...");
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(ActorMoveEvent ev, Player player) {
        GD.Print("PlayerController:OnPlayerMove: Player moving");
        if (player.GetUUID() != GetActor().GetUUID()) {
            GD.Print("PlayerController:OnPlayerMove: Not the right player");
            return;
        }

        Vector3 direction = ev.GetDirection();
        player.SetPosition(player.GetPosition() + direction);
        GD.Print($"PlayerController:OnPlayerMove: Moving player ({player.GetUUID()}) in direction ({direction})  |  New position: {player.GetPosition()}");
    }
}