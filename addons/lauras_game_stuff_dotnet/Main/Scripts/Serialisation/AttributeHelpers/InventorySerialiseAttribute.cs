public class InventorySerialiseAttribute : SerialiseDataAttribute {
    public InventorySerialiseAttribute() : base(InventoryBase.INVENTORY_TAG, "", "", SerialiseHandler.INVENTORY) {}
}