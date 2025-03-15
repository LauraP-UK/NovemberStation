
using Godot;

public interface IContainer {
    public IInventory GetInventory();
    public bool StoreItem(IObjectBase objectBase, Node node);
    public bool RemoveItem(string objectJson);
}