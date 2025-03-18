public interface IQuantitativeInventory : IInventory {
    public int GetMaxQuantity();
    public void SetMaxQuantity(int maxQuantity);
    public int GetUsedQuantity();
    public int GetRemainingQuantity();
}