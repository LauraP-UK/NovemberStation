public class InventoryDataSerialiseAttribute : SerialiseDataAttribute {
    public InventoryDataSerialiseAttribute() : base(InventoryBase.INVENTORY_TAG, "", "", SerialiseHandler.INVENTORY) {}
}