
public abstract class InventoryBase : IInventory {
    protected float _size;
    private readonly SmartDictionary<string, SmartSet<ulong>> _inventory = new(); // MetaTag Type, List of Object IDs

    protected InventoryBase(float size) {
        _size = size;
    }

    public bool AddItem(IObjectBase item) {
        string objectTag = item.GetObjectTag();
        ulong instanceId = item.GetBaseNode3D().GetInstanceId();
        return AddItem(objectTag, instanceId);
    }
    
    public bool AddItem(string metaTag, ulong instanceId) => _inventory.GetOrCompute(metaTag, () => new SmartSet<ulong>()).Add(instanceId);

    public void ClearContents() => _inventory.Clear();
    
    public abstract string GetName();
    public virtual float GetSize() => _size;
}