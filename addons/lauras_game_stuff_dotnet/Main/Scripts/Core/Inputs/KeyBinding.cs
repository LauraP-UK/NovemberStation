using System.Collections.Generic;
using System.Linq;
using Godot;

public class KeyBinding {
    static KeyBinding() => LoadDefaultBindings();

    private static readonly SmartDictionary<InputAction, GameAction.Action> _inputToAction = new();
    public static void BindInput(Key key, GameAction.Action action) => BindInput(InputAction.FromKey(key), action);
    public static void BindInput(MouseButton mouseButton, GameAction.Action action) => BindInput(InputAction.FromMouseButton(mouseButton), action);
    public static void BindInput(InputAction input, GameAction.Action action) => _inputToAction.Add(input, action);
    public static GameAction.Action? GetAction(InputAction input) => _inputToAction.TryGetValue(input, out GameAction.Action action) ? action : null;
    public static IEnumerable<InputAction> GetInputsForAction(GameAction.Action action) => from pair in _inputToAction where pair.Value == action select pair.Key;

    private static void LoadDefaultBindings() {
        BindInput(Key.W, GameAction.Action.MOVE_FORWARD);
        BindInput(Key.S, GameAction.Action.MOVE_BACKWARD);
        BindInput(Key.A, GameAction.Action.MOVE_LEFT);
        BindInput(Key.D, GameAction.Action.MOVE_RIGHT);
        BindInput(Key.Space, GameAction.Action.JUMP);
        BindInput(Key.Ctrl, GameAction.Action.CROUCH);
        BindInput(Key.E, GameAction.Action.USE);
        BindInput(MouseButton.Left, GameAction.Action.USE);
        BindInput(Key.Escape, GameAction.Action.QUIT);
    }
}