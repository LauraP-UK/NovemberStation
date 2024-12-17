
using System;
using System.Collections.Generic;
using Godot;

public class GameActionSpecific {
    private readonly GameAction.Action _action;
    
    private GameActionSpecific(GameAction.Action action) {
        _action = action;
    }
    
    public static GameActionSpecific Create(GameAction.Action action) {
        return new GameActionSpecific(action);
    }
    
    public GameAction.Action GetAction() {
        return _action;
    }
    
    public void CheckAction(Func<Key, bool> testAction, Action<Key> postProcess) {
        IEnumerable<Key> keysForAction = InputController.I().GetKeyBinding().GetKeysForAction(_action);
        foreach (Key key in keysForAction)
            if (testAction(key)) postProcess(key);
    }

    public override int GetHashCode() {
        return _action.GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return _action == ((GameActionSpecific) obj)._action;
    }
}