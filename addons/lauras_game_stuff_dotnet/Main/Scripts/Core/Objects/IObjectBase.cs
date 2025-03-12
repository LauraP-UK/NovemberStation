using System;
using System.Collections.Generic;
using Godot;

public interface IObjectBase {
    public string GetObjectTag();
    public string GetMetaTag();
    public bool TryGetAction(ActionKey actionKey, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> action);
    public List<ActionKey> GetValidActions(ActorBase actorBase, IEventBase ev);
    public Node3D GetBaseNode3D();
    public string GetDisplayName();
    public string GetContext();
}