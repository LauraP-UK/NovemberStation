
using System;
using System.Collections.Generic;
using Godot;

public abstract class InventoryBase : IInventory {
    private readonly SmartDictionary<string, List<string>> _inventory = new(); // ObjectMetaTag Type, List of Object Json
    private readonly SmartSet<Action> _onAdd = new(), _onRemove = new();
    public bool AddItem(IObjectBase item) {
        if (!CanAddItem(item)) return false;
        string objectMetaTag = item.GetObjectTag();
        string jsonData = item.Serialize();
        return AddItem(objectMetaTag, jsonData);
    }
    public bool AddItem(Node3D node) {
        IObjectBase objectClass = GameManager.I().GetObjectClass(node.GetInstanceId());
        if (!CanAddItem(objectClass)) return false;
        string jsonData = objectClass.Serialize();
        string objectMetaTag = objectClass.GetObjectTag();
        return AddItem(objectMetaTag, jsonData);
    }
    public bool AddItem(string objectMetaTag, string jsonData) {
        IObjectBase objectBase = ObjectAtlas.DeserialiseDataWithoutNode(jsonData);
        if (!CanAddItem(objectBase)) return false;
        GetGroup(objectMetaTag).Add(jsonData);
        _onAdd.ForEach(a => a.Invoke());
        return true;
    }
    public abstract bool CanAddItem(IObjectBase item);

    public void RemoveItem(string objectMetaTag, string jsonData) {
        List<string> group = GetGroup(objectMetaTag);
        if (group.Count == 0) {
            GD.PrintErr($"RemoveItem() : No items found with objectMetaTag '{objectMetaTag}'.");
            return;
        }
        group.Remove(jsonData);
        _onRemove.ForEach(a => a.Invoke());
    }
    public bool HasItem(string objectMetaTag) => CountItemType(objectMetaTag) > 0;
    public List<string> GetContents() {
        List<string> contents = new();
        foreach (List<string> group in _inventory.Values) contents.AddRange(group);
        return contents;
    }

    public T GetAs<T>() where T : IInventory {
        if (this is T t) return t;
        GD.PrintErr($"ERROR: InventoryBase.GetAs<T>() : Could not be casted to {typeof(T).Name}!");
        return default;
    }
    public bool IsEmpty() => GetContents().Count == 0;
    public int CountItemType(string objectMetaTag) => GetGroup(objectMetaTag).Count;
    public void ClearContents() => _inventory.Clear();
    private List<string> GetGroup(string key) => _inventory.GetOrCompute(key, () => new List<string>());
    public abstract string GetName();
    protected void OnAdd(Action onAdd) => _onAdd.Add(onAdd);
    protected void OnRemove(Action onRemove) => _onRemove.Add(onRemove);
}