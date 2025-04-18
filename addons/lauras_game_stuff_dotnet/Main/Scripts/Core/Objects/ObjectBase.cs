using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class ObjectBase<T> : ActionHolder, IObjectBase where T : Node3D {
    
    private readonly T _baseNode;
    private readonly string _objectTag;
    [SerialiseData(IObjectBase.GUID_KEY, nameof(SetIdFromString), nameof(SetNewId))]
    private Guid _id;
    
    protected readonly List<IInteractionZone> _interactionZones = []; 
    
    protected ObjectBase(T baseNode, string objectTag) {
        _baseNode = baseNode;
        _objectTag = objectTag;
        SetNewId();
        /*RegisterArbitraryAction("To JSON", 1000, (_, _) => GameManager.IsDebugMode(), (_, ev)  => {
            if (ev is not KeyPressEvent) return;
            GD.Print($"----- {objectTag} JSON -----");
            GD.Print(Serialise());
        });*/
    }
    
    protected void SetId(Guid id) => _id = id;
    protected void SetIdFromString(string id) => _id = Guid.Parse(id);
    protected void SetNewId() => _id = Guid.NewGuid();
    public Guid GetGUID() => _id;

    protected TType FindNode<TType>(string nodePath) where TType : Node => _baseNode.GetNode<TType>(nodePath);
    public T GetBaseNode() => _baseNode;

    public bool IsHeldItem(IHotbarActor actor) => GetGUID().Equals(actor.GetHandItem()?.GetGUID());

    protected void AddInteractionZone(IInteractionZone zone) => _interactionZones.Add(zone);
    public IInteractionZone FindInteractionZoneFor(Node node) {
        if (_interactionZones.Count == 0) return null;
        switch (node) {
            case null: return null;
            case Node3D node3D: {
                IInteractionZone directMatch = _interactionZones.FirstOrDefault(zone => zone.GetContainingNode().Equals(node3D));
                if (directMatch != null) return directMatch;
                break;
            }
        }
        Node current = node;
        while (current != null) {
            if (current is Node3D parent3D) {
                IInteractionZone match = _interactionZones.FirstOrDefault(zone => zone.GetContainingNode().Equals(parent3D));
                if (match != null) return match;
            }
            current = current.GetParent();
        }

        return null;
    }

    public Node3D GetBaseNode3D() => GetBaseNode();
    public virtual bool DisplayContextMenu() => true;
    public string GetObjectTag() => _objectTag;
    public abstract string GetDisplayName();
    public abstract string GetContext();
    public abstract string GetSummary();

    public SmartDictionary<string, SmartSerialData> GetSerialiseData() => ObjectAtlas.GetSerialiseData(GetType());

    public bool BuildFromData(Dictionary<string, object> data) {
        SmartDictionary<string, SmartSerialData> thisData = GetSerialiseData();

        foreach ((string key, SmartSerialData serialData) in thisData) {
            if (!data.TryGetValue(key, out object v)) {
                GD.Print($"WARN: ObjectBase<T>.BuildFromData() : Key not found in data: {key}");
                serialData.InvokeInstanceFallback(this);
                continue;
            }

            try {
                serialData.InvokeInstanceSetter(this, v);
            } catch (Exception e) {
                GD.PrintErr($"WARN: ObjectBase<T>.BuildFromData() : Failed to set {key} to {v} on object {GetDisplayName()}. Using fallback setting...\n{e}");
                serialData.InvokeInstanceFallback(this);
            }
        }
        
        return true;
    }
    public string Serialise() {
        Serialiser.ObjectSaveData data = new() {
            MetaTag = GetObjectTag(),
            TypeID = Items.GetViaPath(GameUtils.FindSceneFilePath(GetBaseNode3D())).GetTypeID(),
            Data = GetSerialiseData().ToDictionary(kvp => kvp.Key, kvp => kvp.Value.InvokeInstanceGetter(this) ?? kvp.Value.GetData())
        };
        return Serialiser.Serialise(data);
    }
}