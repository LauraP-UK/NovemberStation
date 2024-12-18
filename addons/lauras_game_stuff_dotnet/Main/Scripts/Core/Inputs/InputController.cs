using System;
using Godot;

public class InputController {
    private static InputController _instance;

    private readonly KeyBinding _keyBinding;

    public InputController(KeyBinding keyBinding) {
        if (_instance != null) throw new InvalidOperationException("ERROR: InputController.<init> : InputController is a singleton and cannot be instantiated more than once.");
        _instance = this;
        _keyBinding = keyBinding;
        GD.Print("INFO: InputController.<init> : InputController has been instantiated.");
    }

    public static InputController I() {
        if (_instance == null) throw new InvalidOperationException("ERROR: InputController.GetInstance : InputController has not been instantiated yet.");
        return _instance;
    }

    public KeyBinding GetKeyBinding() {
        return _keyBinding;
    }

    public void ProcessInput(InputEvent @event) {
        GD.Print("INFO: InputController._Input() : Input event detected.");
        if (@event is not InputEventKey eventKey) return;
        
        bool isPress = eventKey.Pressed;
        Key key = eventKey.Keycode;
        GD.Print($"INFO: InputController._Input() : Key is {OS.GetKeycodeString(key)}  |  Pressed: {isPress}");
        if (isPress) {
            GameAction.Action? action = _keyBinding.GetAction(key);
            new KeyPressEvent(key).Fire();
            if (action.HasValue) FireGameAction(action.Value);
        } else
            new KeyReleaseEvent(key).Fire();
    }

    private void FireGameAction(GameAction.Action action) {
        switch (action) {
            case GameAction.Action.NONE: return;
            case GameAction.Action.MOVE_FORWARD:
                // Move forward
                break;
            case GameAction.Action.MOVE_BACKWARD:
                // Move backward
                break;
            case GameAction.Action.MOVE_LEFT:
                // Move left
                break;
            case GameAction.Action.MOVE_RIGHT:
                // Move right
                break;
            case GameAction.Action.JUMP:
                // Jump
                break;
            case GameAction.Action.USE:
                // Use
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }
}