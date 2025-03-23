using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

public abstract class InventoryBase : IInventory {
    public const string INVENTORY_TAG = "inventory_data_tag";
    
    private readonly SmartDictionary<string, List<string>> _inventory = new(); // ObjectMetaTag Type, List of Object Json
    private readonly SmartSet<Action> _onAdd = new(), _onRemove = new();

    public AddItemFailCause AddItem(Node3D node) => AddItem(GameManager.I().GetObjectClass(node.GetInstanceId()));
    public AddItemFailCause AddItem(IObjectBase item) {
        AddItemFailCause result = CanAddInternal(item);
        if (result != AddItemFailCause.SUCCESS) return result;
        string jsonData = item.Serialise();
        string objectMetaTag = item.GetObjectTag();
        return AddItemUnchecked(objectMetaTag, jsonData);
    }
    public AddItemFailCause AddItem(string objectMetaTag, string jsonData) {
        IObjectBase objectBase = ObjectAtlas.DeserialiseDataWithoutNode(jsonData);
        AddItemFailCause result = CanAddInternal(objectBase);
        return result != AddItemFailCause.SUCCESS ? result : AddItemUnchecked(objectMetaTag, jsonData);
    }
    
    protected AddItemFailCause AddItemUnchecked(string objectMetaTag, string jsonData) {
        GetGroup(objectMetaTag).Add(jsonData);
        _onAdd.ForEach(a => a.Invoke());
        return AddItemFailCause.SUCCESS;
    }

    public AddItemFailCause CanAddItem(IObjectBase item) {
        string itemJson = item.Serialise();
        return CanAddItem(itemJson);
    }
    public AddItemFailCause CanAddItem(string jsonData) {
        if (this is IFilteredInventory filtered && !filtered.PassesFilters(jsonData)) return AddItemFailCause.FILTER_FAIL;
        return CanAddInternal(jsonData);
    }

    protected abstract AddItemFailCause CanAddInternal(IObjectBase item);
    protected abstract AddItemFailCause CanAddInternal(string jsonData);

    public void RemoveItem(string objectMetaTag, string jsonData) {
        List<string> group = GetGroup(objectMetaTag);
        if (group.Count == 0) {
            GD.PrintErr($"WARN: InventoryBase.RemoveItem() : No items found with objectMetaTag '{objectMetaTag}'.");
            return;
        }

        group.Remove(jsonData);
        _onRemove.ForEach(a => a.Invoke());
    }

    public bool HasItem(string objectMetaTag) => CountItemType(objectMetaTag) > 0;

    public bool HasItem(ItemType itemType) => GetContents()
        .Select(json => Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, json))
        .Any(typeID => typeID == itemType.GetTypeID());

    public List<string> GetContentsOfType(ItemType itemType) => GetContents()
        .Where(json => itemType.GetTypeID() == Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, json))
        .ToList();

    public List<string> GetContents() {
        return _inventory
            .OrderBy(pair => pair.Key)
            .SelectMany(pair => pair.Value)
            .ToList();
    }

    public List<string> GetContentsOfType(string type) => GetGroup(type);

    public T GetAs<T>() where T : IInventory {
        if (this is T t) return t;
        GD.PrintErr($"ERROR: InventoryBase.GetAs<T>() : Could not be casted to {typeof(T).Name}!");
        return default;
    }

    public string Serialise() {
        Dictionary<string, string> inventoryContents = new();
        List<string> items = GetContents();

        for (int i = 0; i < items.Count; i++) inventoryContents[i.ToString()] = items[i];

        Dictionary<string, object> wrapper = new() {
            { INVENTORY_TAG, inventoryContents }
        };

        return JsonSerializer.Serialize(wrapper);
    }
    public void Deserialise(string json) {
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (root.TryGetProperty(INVENTORY_TAG, out JsonElement inventoryElement)) {
            ClearContents();
            foreach (JsonProperty itemProp in inventoryElement.EnumerateObject()) {
                string itemJson = itemProp.Value.GetString();
                if (string.IsNullOrEmpty(itemJson)) continue;

                string metaTag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.META_TAG, itemJson);
                AddItemUnchecked(metaTag, itemJson);
            }
        }
    }

    public bool IsEmpty() => GetContents().Count == 0;
    public int CountItemType(string objectMetaTag) => GetGroup(objectMetaTag).Count;
    public void ClearContents() {
        _onRemove.ForEach(a => a.Invoke());
        _inventory.Clear();
    }

    private List<string> GetGroup(string key) => _inventory.GetOrCompute(key, () => new List<string>());
    public abstract string GetName();
    protected void OnAdd(Action onAdd) => _onAdd.Add(onAdd);
    protected void OnRemove(Action onRemove) => _onRemove.Add(onRemove);
}