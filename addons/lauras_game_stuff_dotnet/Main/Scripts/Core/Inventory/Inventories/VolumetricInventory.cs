using System.Collections.Generic;
using System.Linq;
using Godot;

public class VolumetricInventory : InventoryBase, IVolumetricInventory, IOwnableInventory {
    private float _maxSize;
    private float _lastSize;
    private bool _changedSinceLastCheck = true;
    private IContainer _owner;
    
    public VolumetricInventory(float maxSize, IContainer owner) {
        _maxSize = maxSize;
        _owner = owner;
        OnAdd(() => _changedSinceLastCheck = true);
        OnRemove(() => _changedSinceLastCheck = true);
    }
    
    public override string GetName() => "Volumetric Inventory";
    public float GetMaxSize() => _maxSize;
    public void SetMaxSize(float size) => _maxSize = size;
    public float GetUsedSize() {
        if (!_changedSinceLastCheck) return _lastSize;
        float usedSize = 0;
        foreach (IObjectBase objectData in GetContents().Select(c => ObjectAtlas.DeserialiseDataWithoutNode(c.Item1))) {
            if (objectData is IVolumetricObject volumetricObject) usedSize += volumetricObject.GetSize();
        }
        _lastSize = usedSize;
        _changedSinceLastCheck = false;
        return Mathsf.Round(usedSize, 2);
    }
    public float GetRemainingSize() => Mathsf.Round(GetMaxSize() - GetUsedSize(), 2);
    public AddItemFailCause CanAddItem(Node3D node) {
        IObjectBase objectData = GameManager.GetObjectClass(node.GetInstanceId());
        return CanAddInternal(objectData);
    }
    protected override AddItemFailCause CanAddInternal(IObjectBase item) {
        if (item is not IVolumetricObject volumetricObject) return AddItemFailCause.SUBCLASS_FAIL;
        return GetUsedSize() + volumetricObject.GetSize() <= GetMaxSize() ? AddItemFailCause.SUCCESS : AddItemFailCause.SUBCLASS_FAIL;
    }

    protected override AddItemFailCause CanAddInternal(string jsonData) {
        IObjectBase objectData = ObjectAtlas.DeserialiseDataWithoutNode(jsonData);
        return CanAddInternal(objectData);
    }
    
    public float GetFilledPercentage() => Mathsf.Remap(0.0f, GetMaxSize(), GetUsedSize(), 0.0f, 100.0f);

    public IContainer GetOwner() => _owner;
    public void SetOwner(IContainer owner) => _owner = owner;
}