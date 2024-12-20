
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class GameActionBase {
    private readonly GameAction.Action _action;
    
    protected GameActionBase(GameAction.Action action) {
        _action = action;
    }
    
    public GameAction.Action GetAction() {
        return _action;
    }

    public void RegisterAllListeners() {
        EventManager.RegisterListeners(this);
    }
    
    protected static TEvent GetEvent<TEvent, TContext>(params object[] args) where TEvent : EventBase<TContext> {
        return (TEvent) Activator.CreateInstance(typeof(TEvent), args);
    }

    protected IEnumerable<Key> GetValidKeys() {
        return InputController.I().GetKeyBinding().GetKeysForAction(GetAction());
    }
    
    protected bool IsValidKey(Key key) {
        return GetValidKeys().ToList().Contains(key);
    }

    public override int GetHashCode() {
        return _action.GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return _action == ((GameActionBase) obj)._action;
    }
}