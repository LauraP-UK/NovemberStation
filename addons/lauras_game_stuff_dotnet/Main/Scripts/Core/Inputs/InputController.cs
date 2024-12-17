using System;
using Godot;

public partial class InputController : Node {
    private static InputController _instance;

    private readonly KeyBinding _keyBinding;
    private readonly Func<Key, bool> _isKeyPressed;
    private readonly Func<Key, bool> _isKeyReleased;

    public InputController(KeyBinding keyBinding, Func<Key, bool> isKeyPressed, Func<Key, bool> isKeyReleased) {
        if (_instance != null) throw new InvalidOperationException("ERROR: InputController.<init> : InputController is a singleton and cannot be instantiated more than once.");
        _instance = this;

        _keyBinding = keyBinding;
        _isKeyPressed = isKeyPressed;
        _isKeyReleased = isKeyReleased;
    }

    public static InputController I() {
        if (_instance == null) throw new InvalidOperationException("ERROR: InputController.GetInstance : InputController has not been instantiated yet.");
        return _instance;
    }

    public KeyBinding GetKeyBinding() {
        return _keyBinding;
    }

    public override void _Input(InputEvent @event) {
        
    }

    public void Update() {
        foreach (GameActionSpecific action in GameAction.GetAll()) {
            action.CheckAction(
                key => _isKeyPressed(key) || _isKeyReleased(key),
                key => {
                    if (_isKeyPressed(key)) {
                        new KeyPressEvent(key).Fire();
                        FireGameAction(action.GetAction());
                    }

                    if (_isKeyReleased(key)) new KeyReleaseEvent(key).Fire();
                });
        }
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