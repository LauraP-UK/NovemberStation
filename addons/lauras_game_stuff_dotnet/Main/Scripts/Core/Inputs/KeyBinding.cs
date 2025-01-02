using System.Collections.Generic;
using System.Linq;
using Godot;

public class KeyBinding {
    
    private static readonly AutoDictionary<InputAction, GameAction.Action> _inputToAction = new();
    
    public static void BindInput(Key key, GameAction.Action action) => BindInput(InputAction.FromKey(key), action);
    public static void BindInput(MouseButton mouseButton, GameAction.Action action) => BindInput(InputAction.FromMouseButton(mouseButton), action);
    public static void BindInput(InputAction input, GameAction.Action action) => _inputToAction.Add(input, action);
    public static GameAction.Action? GetAction(InputAction input) => _inputToAction.TryGetValue(input, out GameAction.Action action) ? action : null;
    public static IEnumerable<InputAction> GetInputsForAction(GameAction.Action action) => from pair in _inputToAction where pair.Value == action select pair.Key;
}