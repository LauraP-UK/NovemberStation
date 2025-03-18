
using System.Collections.Generic;
using Godot;

public interface IInventory {
    public string GetName();
    public AddItemFailCause AddItem(IObjectBase item);
    public AddItemFailCause AddItem(Node3D node);
    public AddItemFailCause AddItem(string objectMetaTag, string jsonData);
    public AddItemFailCause CanAddItem(IObjectBase item);
    public AddItemFailCause CanAddItem(string jsonData);
    public void RemoveItem(string objectMetaTag, string jsonData);
    public int CountItemType(string objectMetaTag);
    public bool HasItem(string objectMetaTag);
    public bool HasItem(ItemType itemType);
    public bool IsEmpty();
    public List<string> GetContents();
    public List<string> GetContentsOfType(string type);
    public List<string> GetContentsOfType(ItemType type);
    public void ClearContents();
    public T GetAs<T>() where T : IInventory;
    public string Serialise();
    public void Deserialise(string json);
}