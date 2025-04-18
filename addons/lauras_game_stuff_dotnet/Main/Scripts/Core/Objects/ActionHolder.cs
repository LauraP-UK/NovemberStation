
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ActionHolder : IActionHolder {
    private readonly SmartDictionary<ActionKey, (Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> run)> _actions = new();
    
    protected void RegisterAction<TAction>(Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> action) where TAction : IObjectAction =>
        _actions[new ActionKey(typeof(TAction))] = (test, action);
    
    protected void RegisterArbitraryAction(string name, int index, Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> action) {
        ActionAtlas.RegisterCustom(name, index);
        _actions[new ActionKey(name)] = (test, action);
    }

    public bool TryGetAction(ActionKey actionKey, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> action) {
        if (_actions.TryGetValue(actionKey, out (Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> run) actions)) {
            test = actions.test;
            action = actions.run;
            return true;
        }
        test = null;
        action = null;
        return false;
    }

    public List<ActionKey> GetValidActions(ActorBase actorBase, IEventBase ev) {
        List<ActionKey> validActions = new();
        List<KeyValuePair<ActionKey,(Func<ActorBase,IEventBase,bool> test, Action<ActorBase,IEventBase> run)>> sorted = _actions
            .OrderBy(a => ActionAtlas.GetActionIndex(a.Key))
            .ToList();

        foreach ((ActionKey key, (Func<ActorBase, IEventBase, bool> test, _)) in sorted)
            if (test.Invoke(actorBase, ev)) validActions.Add(key);

        return validActions;
    }
    
    public bool TestAction<TAction>(ActorBase actorBase, IEventBase ev) where TAction : IObjectAction {
        (Func<ActorBase,IEventBase,bool> test, Action<ActorBase,IEventBase> run) pair = _actions.GetOrDefault(new ActionKey(typeof(TAction)), (null,null));
        return pair.test != null && pair.test.Invoke(actorBase, ev);
    }
}