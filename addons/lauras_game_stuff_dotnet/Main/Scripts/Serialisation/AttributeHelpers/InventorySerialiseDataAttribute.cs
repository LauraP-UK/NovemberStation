public class InventorySerialiseDataAttribute : SerialiseDataAttribute {
    public InventorySerialiseDataAttribute() : base(InventoryBase.INVENTORY_TAG, "", "", SerialiseHandler.INVENTORY) {}
}