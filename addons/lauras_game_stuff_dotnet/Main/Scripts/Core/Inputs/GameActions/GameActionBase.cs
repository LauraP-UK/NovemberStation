
using System;

public abstract class GameActionBase {
    private readonly GameAction.Action _action;
    
    protected GameActionBase(GameAction.Action action) {
        _action = action;
    }
    
    public GameAction.Action GetAction() {
        return _action;
    }
    
    protected static TEvent GetEvent<TEvent, TContext>(params object[] args) where TEvent : EventBase<TContext> {
        return (TEvent) Activator.CreateInstance(typeof(TEvent), args);
    }

    public abstract void RegisterListeners();

    public override int GetHashCode() {
        return _action.GetHashCode();
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return _action == ((GameActionBase) obj)._action;
    }
}