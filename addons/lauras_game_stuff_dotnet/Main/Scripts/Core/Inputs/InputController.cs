using System;
using Godot;

public class InputController {
    private static InputController _instance;

    private readonly KeyBinding _keyBinding;

    public InputController(KeyBinding keyBinding) {
        if (_instance != null) throw new InvalidOperationException("ERROR: InputController.<init> : InputController is a singleton and cannot be instantiated more than once.");
        _instance = this;
        _keyBinding = keyBinding;
    }

    public static InputController I() {
        if (_instance == null) throw new InvalidOperationException("ERROR: InputController.GetInstance : InputController has not been instantiated yet.");
        return _instance;
    }

    public KeyBinding GetKeyBinding() {
        return _keyBinding;
    }

    public void ProcessInput(InputEvent @event) {
        if (@event is not InputEventKey eventKey) return;
        if (eventKey.Pressed)
            new KeyPressEvent(eventKey.Keycode).Fire();
        else
            new KeyReleaseEvent(eventKey.Keycode).Fire();
    }
}