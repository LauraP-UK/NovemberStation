
using Godot;

public interface IContainer {
    public IInventory GetInventory();
    public string GetName();
    public AddItemFailCause StoreItem(ItemType itemType);
    public AddItemFailCause StoreItem(IObjectBase objectBase, Node node);
    public AddItemFailCause StoreItem(string objectMetaTag, string objectJson);
    public bool RemoveItem(string objectJson);
}