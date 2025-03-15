
public interface IVolumetricInventory : IInventory {
    public float GetMaxSize();
    public float GetUsedSize();
    public float GetRemainingSize();
}