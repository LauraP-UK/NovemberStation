using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class GameActionBase : Listener {
    private readonly GameAction.Action _action;
    
    protected GameActionBase(GameAction.Action action) {
        _action = action;
    }
    
    public GameAction.Action GetAction() => _action;
    protected IEnumerable<InputAction> GetValidKeys() => KeyBinding.GetInputsForAction(GetAction());
    protected bool IsValidInput(InputAction input) => GetValidKeys().ToList().Any(registeredInputs => registeredInputs.Equals(input));
    protected bool IsValidInput(Key input) => GetValidKeys().ToList().Any(registeredInputs => registeredInputs.Is(input));
    protected bool IsValidInput(MouseButton input) => GetValidKeys().ToList().Any(registeredInputs => registeredInputs.Is(input));

    public override int GetHashCode() => _action.GetHashCode();

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return _action == ((GameActionBase) obj)._action;
    }
}