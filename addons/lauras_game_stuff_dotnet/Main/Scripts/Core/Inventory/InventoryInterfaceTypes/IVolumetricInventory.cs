using Godot;

public interface IVolumetricInventory : IInventory {
    public float GetMaxSize();
    public float GetUsedSize();
    public float GetRemainingSize();
    public bool CanAddItem(IObjectBase item);
    public bool CanAddItem(Node3D node);
}