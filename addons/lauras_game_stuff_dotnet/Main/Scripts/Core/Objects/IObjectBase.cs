using System;
using System.Collections.Generic;
using Godot;

public interface IObjectBase {
    public const string GUID_KEY = "guid";
    public Guid GetGUID();
    public string GetObjectTag();
    public bool TryGetAction(ActionKey actionKey, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> action);
    public List<ActionKey> GetValidActions(ActorBase actorBase, IEventBase ev);
    public bool TestAction<TAction>(ActorBase actorBase, IEventBase ev) where TAction : IObjectAction;
    public IInteractionZone FindInteractionZoneFor(Node node);
    public Node3D GetBaseNode3D();
    public bool DisplayContextMenu();
    public string GetDisplayName();
    public string GetContext();
    public string GetSummary();
    public SmartDictionary<string, SmartSerialData> GetSerialiseData();
    public bool BuildFromData(Dictionary<string, object> data);
    public string Serialise();
}