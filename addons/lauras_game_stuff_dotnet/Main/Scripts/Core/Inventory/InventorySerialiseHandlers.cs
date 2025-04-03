using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;

public static class InventorySerialiseHandlers {
    public static object GetInventory(object instance) {
        if (instance is not IContainer container)
            throw new InvalidOperationException($"Cannot get inventory from non-IContainer instance. Got {(instance == null ? "NULL" : instance.GetType().Name)}.");
        
        return container.GetInventory().SerialiseToDict();
    }
    public static void SetInventory(object instance, object value) {
        if (instance is not IContainer container) {
            GD.PrintErr($"ERROR: InventorySerialiseHandlers.SetInventory(): Instance is not IContainer. Got {(instance == null ? "null" : instance.GetType())}");
            return;
        }

        if (value is Dictionary<string, object> rawDict) {
            // This happens if you're deserializing into Dictionary<string, object>
            Dictionary<string, string> items = rawDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
            container.GetInventory().DeserialiseFromDict(items);
            return;
        }

        try {
            // Fallback attempt using JsonSerializer — might catch certain edge cases
            string json = JsonSerializer.Serialize(value);
            Dictionary<string, string> parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            container.GetInventory().DeserialiseFromDict(parsed);
        } catch (Exception e) {
            GD.PrintErr($"ERROR: InventorySerialiseHandlers.SetInventory(): Could not process inventory. {e}");
        }
    }

    public static void FallbackInventory(object instance) {
        if (instance is not IContainer container) return;
        IInventory inv = container.GetInventory();
        inv.ClearContents();
    }
}