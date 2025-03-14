
using Godot;

public interface IContainer {
    public IInventory GetInventory();
    public bool StoreItem(IObjectBase objectBase, Node node);
    public bool RemoveItem(string objectJson);
    bool TryGetInventory<TInventory>(out TInventory inventory) where TInventory : class, IInventory {
        inventory = GetInventory() as TInventory;
        return inventory != null;
    }
}