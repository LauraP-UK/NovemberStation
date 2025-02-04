
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class ObjectBase<T> : IObjectBase where T : Node3D {
    private readonly T _baseNode;
    private readonly SmartDictionary<Type, (Func<ActorBase, IEventBase, bool> Test, Action<ActorBase, IEventBase> Run)> _actions = new();
    private readonly string _objectTag, _metaTag;
    
    protected ObjectBase(T baseNode, string objectTag, string metaTag) {
        _baseNode = baseNode;
        _objectTag = objectTag;
        _metaTag = metaTag;
    }

    protected TType FindNode<TType>(string nodePath) where TType : Node => _baseNode.GetNode<TType>(nodePath);
    public T GetBaseNode() => _baseNode;
    protected void RegisterAction<TAction>(Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> action) where TAction : IObjectAction => _actions[typeof(TAction)] = (test, action);

    public bool TryGetAction(Type actionType, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> action) {
        if (_actions.TryGetValue(actionType, out (Func<ActorBase, IEventBase, bool> Test, Action<ActorBase, IEventBase> Run) actions)) {
            test = actions.Test;
            action = actions.Run;
            return true;
        }
        test = null;
        action = null;
        return false;
        
    }

    public List<Type> GetValidActions(ActorBase actorBase, IEventBase ev) {
        List<Type> validActions = new();
        List<KeyValuePair<Type,(Func<ActorBase,IEventBase,bool> Test, Action<ActorBase,IEventBase> Run)>> sorted = _actions
            .OrderBy(a => ActionAtlas.GetActionIndex(a.Key))
            .ToList();
        
        foreach ((Type key, (Func<ActorBase, IEventBase, bool> test, _)) in sorted)
            if (test.Invoke(actorBase, ev)) validActions.Add(key);
        return validActions;
    }

    public Node3D GetBaseNode3D() => GetBaseNode();
    public string GetObjectTag() => _objectTag;
    public string GetMetaTag() => _metaTag;
}