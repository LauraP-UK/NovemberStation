
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class ObjectBase<T> : IObjectBase where T : Node3D {
    
    private readonly T _baseNode;
    private readonly SmartDictionary<ActionKey, (Func<ActorBase, IEventBase, bool> test, Action<ActorBase, IEventBase> run)> _actions = new();
    private readonly string _objectTag;
    
    protected ObjectBase(T baseNode, string objectTag) {
        _baseNode = baseNode;
        _objectTag = objectTag;
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
    public abstract string GetDisplayName();
    public abstract string GetContext();
    public abstract SmartDictionary<string, SmartSerialData> GetSerialiseData();
    public bool BuildFromData(Dictionary<string, object> data) {
        SmartDictionary<string, SmartSerialData> thisData = GetSerialiseData();
        
        foreach ((string key, SmartSerialData serialData) in thisData) {
            if (!data.TryGetValue(key, out object v)) {
                GD.Print($"WARN: ObjectBase<T>.BuildFromData() : Key not found in data: {key}");
                serialData.GetFallback().Invoke();
                continue;
            }

            try {
                serialData.GetSetter().Invoke(v);
            } catch (Exception e) {
                GD.PrintErr($"WARN: ObjectBase<T>.BuildFromData() : Failed to set {key} to {v} on object {GetDisplayName()}. Using fallback setting...\n{e}");
                serialData.GetFallback().Invoke();
            }
        }
        
        return true;
    }
    public string Serialize() {
        Serialiser.ObjectSaveData data = new() {
            MetaTag = GetObjectTag(),
            TypeID = Items.GetViaPath(GameUtils.FindSceneFilePath(GetBaseNode3D())).GetTypeID(),
            Data = GetSerialiseData().ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetData())
        };
        return Serialiser.Serialise(data);
    }
}