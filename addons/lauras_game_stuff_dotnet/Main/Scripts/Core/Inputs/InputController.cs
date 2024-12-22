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
        if (@event is InputEventKey eventKey) {
            if (eventKey.Pressed)
                new KeyPressEvent(eventKey.Keycode).Fire();
            else
                new KeyReleaseEvent(eventKey.Keycode).Fire();
        }
        else if (@event is InputEventMouseButton eventMouseButton) {
            MouseButton button = eventMouseButton.ButtonIndex;
            bool pressed = eventMouseButton.Pressed;
            new MouseClickEvent(button, pressed, eventMouseButton.Position).Fire();
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            new MouseMoveEvent(eventMouseMotion.Position, eventMouseMotion.Relative).Fire();
        }
    }
}