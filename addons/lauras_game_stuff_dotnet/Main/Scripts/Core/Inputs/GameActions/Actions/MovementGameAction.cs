using Godot;
using NovemberStation.Main;

public class MovementGameAction : GameActionBase {
    
    private readonly Vector3 _offset;
    
    public MovementGameAction(GameAction.Action action, Vector3 offset) : base(action) {
        _offset = offset;
    }

    [EventListener]
    private void OnMoveKeyPress(KeyPressEvent ev, Key context) {
        if (!IsValidKey(context)) return;
        GD.Print($"RESULT: MovementGameAction : {context}");
        ActorMoveEvent moveEvent = new();
        moveEvent.SetDirection(_offset);
        moveEvent.SetActor(TestScript.I().GetPlayer());
        moveEvent.Fire();
    }
}