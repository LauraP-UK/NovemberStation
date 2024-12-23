using Godot;
using NovemberStation.Main;

public class MovementGameAction : GameActionBase {
    
    private readonly Vector3 _offset;
    private bool _isKeyPressed;
    
    public MovementGameAction(GameAction.Action action, Vector3 offset) : base(action) {
        _offset = offset;
    }

    [EventListener]
    private void OnMoveKeyPress(KeyPressEvent ev, Key context) {
        if (!IsValidKey(context)) return;
        _isKeyPressed = true;
    }

    [EventListener]
    private void OnMoveKeyRelease(KeyReleaseEvent ev, Key context) {
        if (!IsValidKey(context)) return;
        _isKeyPressed = false;
    }

    public void Update() {
        if (!_isKeyPressed) return;
        PlayerMoveEvent moveEvent = new();
        moveEvent.SetDirection(_offset);
        moveEvent.SetActor(TestScript.I().GetPlayer());
        moveEvent.Fire();
    }
}