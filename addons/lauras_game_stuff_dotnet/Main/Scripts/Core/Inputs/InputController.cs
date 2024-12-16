using System;

public class InputController {
    private readonly KeyBinding _keyBinding;
    private readonly Func<string, bool> _isKeyPressed;
    private readonly Func<string, bool> _isKeyReleased;

    public InputController(KeyBinding keyBinding, Func<string, bool> isKeyPressed, Func<string, bool> isKeyReleased) {
        _keyBinding = keyBinding;
        
    }
}
