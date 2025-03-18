
public interface IVolumetricInventory : IInventory {
    public float GetMaxSize();
    public void SetMaxSize (float value);
    public float GetUsedSize();
    public float GetRemainingSize();
}