using Godot;
using NovemberStation.Main;

public class CrouchGameAction : GameActionBase {
    
    private bool _isKeyPressed;
    public CrouchGameAction(GameAction.Action action) : base(action) { }
    
    [EventListener]
    protected void OnMoveKeyPress(KeyPressEvent ev, Key context) {
        if (!IsValidInput(context) || _isKeyPressed) return;
        _isKeyPressed = true;
        Fire();
    }

    [EventListener]
    protected void OnMoveKeyRelease(KeyReleaseEvent ev, Key context) {
        if (!IsValidInput(context) || !_isKeyPressed) return;
        _isKeyPressed = false;
        Fire();
    }

    private void Fire() {
        ActorCrouchEvent crouchEvent = new(TestScript.I().GetPlayer());
        crouchEvent.SetStartCrouch(IsKeyPressed());
        crouchEvent.Fire();
    }
    
    public bool IsKeyPressed() => _isKeyPressed;
}