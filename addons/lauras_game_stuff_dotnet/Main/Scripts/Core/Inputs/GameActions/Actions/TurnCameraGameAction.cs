
using Godot;
using NovemberStation.Main;

public class TurnCameraGameAction : GameActionBase {
    public TurnCameraGameAction(GameAction.Action action) : base(action) { }
    
    [EventListener]
    private void OnMouseMove(MouseMoveEvent ev, Vector2 delta) {
        PlayerMoveEvent moveEvent = new();
        moveEvent.SetTurnDelta(delta);
        moveEvent.SetActor(GameManager.I().GetPlayer());
        moveEvent.Fire();
    }
}