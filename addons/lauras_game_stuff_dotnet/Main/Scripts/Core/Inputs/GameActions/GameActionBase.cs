
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class GameActionBase : Listener {
    private readonly GameAction.Action _action;
    
    protected GameActionBase(GameAction.Action action) {
        _action = action;
    }
    
    public GameAction.Action GetAction() => _action;
    protected IEnumerable<Key> GetValidKeys() => InputController.I().GetKeyBinding().GetKeysForAction(GetAction());
    protected bool IsValidKey(Key key) => GetValidKeys().ToList().Contains(key);
    public override int GetHashCode() => _action.GetHashCode();

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return _action == ((GameActionBase) obj)._action;
    }
}