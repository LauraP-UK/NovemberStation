
using System;
using Godot;

public static class InventorySerialiseHandlers {
    public static object GetInventory(object instance) {
        if (instance is not IContainer container)
            throw new InvalidOperationException($"Cannot get inventory from non-IContainer instance. Got {(instance == null ? "NULL" : instance.GetType().Name)}.");
        return container.GetInventory().Serialise();
    }
    public static void SetInventory(object instance, object jsonValue) {
        if (instance is not IContainer container) {
            GD.PrintErr($"ERROR: InventorySerialiseHandlers.SetInventory(): Instance is not IContainer. Got {instance.GetType()}");
            return;
        }
        
        if (jsonValue is not string json) {
            GD.PrintErr($"ERROR: InventorySerialiseHandlers.SetInventory(): Invalid JSON. Got {(jsonValue == null ? "NULL" : jsonValue.GetType())}");
            return;
        }

        IInventory inv = container.GetInventory();
        inv.Deserialise(json);
    }

    public static void FallbackInventory(object instance) {
        if (instance is not IContainer container) return;
        IInventory inv = container.GetInventory();
        inv.ClearContents();
    }
}