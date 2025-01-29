
using Godot;

public class UseGameAction : GameActionBase {

    private ActionBase _lastAction;
    public UseGameAction(GameAction.Action action) : base(action) { }

    [EventListener]
    private void OnMouseUsePress(MouseInputEvent ev, Vector2 coords) {
        if (!IsValidInput(ev.GetMouseButton()) || ev.IsCaptured()) return;
        
        Player player = GameManager.I().GetPlayer();
        PlayerController controller = player.GetController<PlayerController>();
        
        ActionBase action = controller.GetCurrentContextAction() ?? _lastAction;
        Node3D obj = controller.GetContextObject();

        if (action == null || obj == null) return;
        action.Invoke(player, obj, ev);
        _lastAction = action;
    }
    
    private void FireEmptyEvent() {
        ActorPickUpEvent pickUpEvent = new();
        pickUpEvent.SetActor(GameManager.I().GetPlayer());
        pickUpEvent.SetItem(null);
        pickUpEvent.Fire();
    }
}