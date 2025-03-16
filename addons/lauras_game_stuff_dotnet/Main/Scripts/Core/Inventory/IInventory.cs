
using System.Collections.Generic;
using Godot;

public interface IInventory {
    public string GetName();
    public bool AddItem(IObjectBase item);
    public bool AddItem(Node3D node);
    public bool AddItem(string objectMetaTag, string jsonData);
    public bool CanAddItem(IObjectBase item);
    public bool CanAddItem(string jsonData);
    public void RemoveItem(string objectMetaTag, string jsonData);
    public int CountItemType(string objectMetaTag);
    public bool HasItem(string objectMetaTag);
    public bool IsEmpty();
    public List<string> GetContents();
    public List<string> GetContentsOfType(string type);
    public void ClearContents();
    public T GetAs<T>() where T : IInventory;
}