using Godot;

public class MovementGameAction : GameActionBase {
    public MovementGameAction(GameAction.Action action) : base(action) {
    }

    [EventListener]
    private void OnMoveKeyPress(KeyPressEvent ev, Key context) {
        if (IsValidKey(context)) {
            GD.Print($"RESULT: MovementGameAction : {context}");
        }
    }
}