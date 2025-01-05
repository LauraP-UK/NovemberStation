using Godot;

public class MovementGameAction : GameActionBase {
    
    private readonly Vector3 _offset;
    private bool _isKeyPressed;
    
    public MovementGameAction(GameAction.Action action, Vector3 offset) : base(action) {
        _offset = offset;
        MovementActionTracker.RegisterMovementAction(this);
    }

    [EventListener]
    protected void OnMoveKeyPress(KeyPressEvent ev, Key context) {
        if (!IsValidInput(context)) return;
        _isKeyPressed = true;
    }

    [EventListener]
    protected void OnMoveKeyRelease(KeyReleaseEvent ev, Key context) {
        if (!IsValidInput(context)) return;
        _isKeyPressed = false;
    }
    
    public Vector3 GetOffset() => _offset;
    public bool IsKeyPressed() => _isKeyPressed;
}