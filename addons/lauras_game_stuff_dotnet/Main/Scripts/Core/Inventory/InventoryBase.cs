using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

public abstract class InventoryBase : IInventory {
    public const string INVENTORY_TAG = "inventory_data_tag";
    
    private readonly SmartDictionary<string, List<(string json, Guid guid)>> _inventory = new(); // ObjectMetaTag Type, List of Object Json
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
        string guidString = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, jsonData);
        GetGroup(objectMetaTag).Add((jsonData, Guid.Parse(guidString)));
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
        List<(string,Guid)> group = GetGroup(objectMetaTag);
        if (group.Count == 0) {
            GD.PrintErr($"WARN: InventoryBase.RemoveItem() : No items found with objectMetaTag '{objectMetaTag}'.");
            return;
        }
        string guidString = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, jsonData);
        group.Remove((jsonData, Guid.Parse(guidString)));
        _onRemove.ForEach(a => a.Invoke());
    }
    
    public bool UpdateItem(string updatedJson) {
        string updatedGuid = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, updatedJson);
        if (string.IsNullOrEmpty(updatedGuid)) {
            GD.PrintErr("WARN: InventoryBase.UpdateItem() : Invalid or missing GUID in updatedJson.");
            return false;
        }
        
        string tag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.META_TAG, updatedJson);
        
        List<(string,Guid)> contents = GetGroup(tag);
        for (int i = 0; i < contents.Count; i++) {
            Guid existingGuid = contents[i].Item2;
            if (existingGuid.ToString() != updatedGuid) continue;
            contents[i] = (updatedJson,existingGuid);
            return true;
        }
        return false;
    }

    public bool HasItem(string objectMetaTag) => CountItemType(objectMetaTag) > 0;

    public bool HasItem(ItemType itemType) => GetContents()
        .Select(json => Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, json.Item1))
        .Any(typeID => typeID == itemType.GetTypeID());

    public List<(string,Guid)> GetContentsOfType(ItemType itemType) => GetContents()
        .Where(item => itemType.GetTypeID() == Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, item.Item1))
        .ToList();

    public string GetViaGUID(Guid id) {
        foreach ((string json, Guid guid) in GetContents())
            if (id.Equals(guid)) return json;
        return null;
    }

    public List<(string,Guid)> GetContents() {
        return _inventory
            .OrderBy(pair => pair.Key)
            .SelectMany(pair => pair.Value)
            .ToList();
    }

    public List<(string,Guid)> GetContentsOfType(string type) => GetGroup(type);

    public T GetAs<T>() where T : IInventory {
        if (this is T t) return t;
        GD.PrintErr($"ERROR: InventoryBase.GetAs<T>() : Could not be casted to {typeof(T).Name}!");
        return default;
    }

    public string Serialise() {
        Dictionary<string, string> inventoryContents = new();
        List<(string,Guid)> items = GetContents();

        for (int i = 0; i < items.Count; i++) inventoryContents[i.ToString()] = items[i].Item1;

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

    public Dictionary<string, string> SerialiseToDict() {
        Dictionary<string, string> inventoryContents = new();
        List<(string,Guid)> items = GetContents();

        for (int i = 0; i < items.Count; i++) inventoryContents[i.ToString()] = items[i].Item1;

        return inventoryContents;
    }
    public void DeserialiseFromDict(Dictionary<string, string> data) {
        ClearContents();
        foreach (KeyValuePair<string, string> entry in data) {
            ObjectAtlas.CreatedObject created = ObjectAtlas.CreatedObjectFromJson(entry.Value);
            if (created.Success) AddItem(created.Object);
            created.Node.QueueFree();
        }
    }

    public bool IsEmpty() => GetContents().Count == 0;
    public int CountItemType(string objectMetaTag) => GetGroup(objectMetaTag).Count;
    public void ClearContents() {
        _onRemove.ForEach(a => a.Invoke());
        _inventory.Clear();
    }

    private List<(string,Guid)> GetGroup(string key) => _inventory.GetOrCompute(key, () => new List<(string,Guid)>());
    public abstract string GetName();
    protected void OnAdd(Action onAdd) => _onAdd.Add(onAdd);
    protected void OnRemove(Action onRemove) => _onRemove.Add(onRemove);
}