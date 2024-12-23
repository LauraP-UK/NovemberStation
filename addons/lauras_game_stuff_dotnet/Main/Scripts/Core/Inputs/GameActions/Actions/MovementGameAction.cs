using Godot;

public class MovementGameAction : GameActionBase {
    
    private readonly Vector3 _offset;
    private bool _isKeyPressed;
    
    public MovementGameAction(GameAction.Action action, Vector3 offset) : base(action) {
        _offset = offset;
        MovementActionTracker.RegisterMovementAction(this);
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
    
    public Vector3 GetOffset() => _offset;
    public bool IsKeyPressed() => _isKeyPressed;
}