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
    public float GetUsedSize() {
        if (!_changedSinceLastCheck) return _lastSize;
        float usedSize = 0;
        foreach (IObjectBase objectData in GetContents().Select(ObjectAtlas.DeserialiseDataWithoutNode)) {
            if (objectData is IVolumetricObject volumetricObject) usedSize += volumetricObject.GetSize();
        }
        _lastSize = usedSize;
        _changedSinceLastCheck = false;
        return usedSize;
    }
    public float GetRemainingSize() => GetMaxSize() - GetUsedSize();
    public bool CanAddItem(Node3D node) {
        IObjectBase objectData = GameManager.I().GetObjectClass(node.GetInstanceId());
        return CanAddItem(objectData);
    }
    public override bool CanAddItem(IObjectBase item) {
        if (item is not IVolumetricObject volumetricObject) return false;
        return GetUsedSize() + volumetricObject.GetSize() <= GetMaxSize();
    }
    
    public float GetFilledPercentage() => Mathsf.Remap(0.0f, GetMaxSize(), GetUsedSize(), 0.0f, 100.0f);

    public void PrintContents() {
        GD.Print($"Total size: {GetMaxSize()}  |  Used size: {GetUsedSize()}  |  Remaining size: {GetRemainingSize()}  |  Filled percentage: {GetFilledPercentage()}%");
        List<string> contents = GetContents();
        int counter = 0;
        contents.ForEach(o => {
            GD.Print($"Slot {counter}: {o}");
            counter++;
        });
    }

    public IContainer GetOwner() => _owner;
    public void SetOwner(IContainer owner) => _owner = owner;
}