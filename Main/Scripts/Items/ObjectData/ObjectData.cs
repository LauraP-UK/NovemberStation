
using System;
using System.Linq;
using Godot;

public class ObjectData {
    private readonly SmartSet<ActionBase> _actions = new();
    private readonly string _metaTag;
    public ObjectData(string metaTag, Action<ObjectData> onInit) {
        _metaTag = metaTag;
        onInit?.Invoke(this);
    }

    public void AddAction(ActionBase action) => _actions.Add(action);
    public void InvokeAction<T>(string actionName, ActorBase actorBase, T node) where T : Node3D => _actions.First(a => a.GetActionName() == actionName).Invoke(actorBase, node, this);
    public SmartSet<ActionBase> GetActions() => new(_actions);
    public string GetMetaTag() => _metaTag;
}