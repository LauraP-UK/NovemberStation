
using Godot;

public class PlayerController : ControllerBase {

    public PlayerController(Player player) : base(player) {}

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnPlayerMove(ActorMoveEvent ev, Player player) {
        GD.Print("PlayerController:OnPlayerMove: Player moving");
        if (player.GetUUID() != GetActor().GetUUID()) {
            GD.Print("PlayerController:OnPlayerMove: Not the right player");
            return;
        }

        Vector3 direction = ev.GetDirection();

        GD.Print($"PlayerController:OnPlayerMove: Moving player ({player.GetUUID()}) in direction ({direction})");
    }
}