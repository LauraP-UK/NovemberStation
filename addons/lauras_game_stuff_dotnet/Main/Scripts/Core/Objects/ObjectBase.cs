
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class ObjectBase<T> : IObjectBase where T : Node3D {
    private readonly T _baseNode;
    private readonly SmartDictionary<ActionKey, (Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> run)> _actions = new();
    private readonly string _objectTag, _metaTag;
    
    protected ObjectBase(T baseNode, string objectTag, string metaTag) {
        _baseNode = baseNode;
        _objectTag = objectTag;
        _metaTag = metaTag;
    }

    protected TType FindNode<TType>(string nodePath) where TType : Node => _baseNode.GetNode<TType>(nodePath);
    public T GetBaseNode() => _baseNode;
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

    public Node3D GetBaseNode3D() => GetBaseNode();
    public string GetObjectTag() => _objectTag;
    public string GetMetaTag() => _metaTag;
    public abstract string GetDisplayName();
    public abstract string GetContext();
    public abstract SmartDictionary<string, (Variant, Action<Variant>)> GetSerializeData();

    public bool BuildFromData(SmartDictionary<string, (Variant, Action<Variant>)> data) {
        SmartSet<string> keys = new (GetSerializeData().Keys);
        if (!keys.SetEquals(data.Keys)) return false;
        foreach ((string _, (Variant variant, Action<Variant> action)) in data) action.Invoke(variant);
        return true;
    }
}