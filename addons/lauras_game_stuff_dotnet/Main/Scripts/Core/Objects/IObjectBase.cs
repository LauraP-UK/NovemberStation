using System;
using System.Collections.Generic;
using Godot;

public interface IObjectBase {
    public string GetObjectTag();
    public string GetMetaTag();
    public bool TryGetAction(Type actionType, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> action);
    public List<Type> GetValidActions(ActorBase actorBase, IEventBase ev);
    public Node3D GetBaseNode3D();
    public string GetDisplayName();
    public string GetContext();
}