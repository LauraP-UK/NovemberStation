using Godot;

public class JumpGameAction : GameActionBase {
    public JumpGameAction(GameAction.Action action) : base(action) { }
    
    [EventListener]
    private void OnJumpKeyPress(KeyPressEvent ev, Key key) {
        if (!IsValidKey(key)) return;
        new PlayerJumpEvent().Fire();
    }
}