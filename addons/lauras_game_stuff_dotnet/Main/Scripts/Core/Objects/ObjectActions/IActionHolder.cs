using System;
using System.Collections.Generic;

public interface IActionHolder {
    public bool TryGetAction(ActionKey actionKey, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> action);
    public List<ActionKey> GetValidActions(ActorBase actorBase, IEventBase ev);
    public bool TestAction<TAction>(ActorBase actorBase, IEventBase ev) where TAction : IObjectAction;
}