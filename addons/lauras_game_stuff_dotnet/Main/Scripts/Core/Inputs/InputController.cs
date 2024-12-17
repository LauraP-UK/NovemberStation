using System;

public class InputController {
    private readonly KeyBinding _keyBinding;
    private readonly Func<string, bool> _isKeyPressed;
    private readonly Func<string, bool> _isKeyReleased;

    public InputController(KeyBinding keyBinding, Func<string, bool> isKeyPressed, Func<string, bool> isKeyReleased) {
        _keyBinding = keyBinding;
        _isKeyPressed = isKeyPressed;
        _isKeyReleased = isKeyReleased;
    }

    public void Update() {
        foreach (string key in _keyBinding.GetKeysForAction(GameAction.MOVE_FORWARD)) {
            if (_isKeyPressed(key)) {
                
            }
        }
    }
}
