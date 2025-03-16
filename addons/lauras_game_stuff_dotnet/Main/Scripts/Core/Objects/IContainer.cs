
using Godot;

public interface IContainer {
    public IInventory GetInventory();
    public bool StoreItem(IObjectBase objectBase, Node node);
    public bool StoreItem(string objectMetaTag, string objectJson);
    public bool RemoveItem(string objectJson);
}