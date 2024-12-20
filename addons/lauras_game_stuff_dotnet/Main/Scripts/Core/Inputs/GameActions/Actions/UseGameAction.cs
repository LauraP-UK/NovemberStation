
using Godot;

public class UseGameAction : GameActionBase {
    public UseGameAction(GameAction.Action action) : base(action) { }
    
    [EventListener]
    private void OnUseKeyPress(KeyPressEvent ev, Key key) {
        if (!IsValidKey(key)) return;
        GD.Print($"RESULT: Use! : {key}");
    }
}