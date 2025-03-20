using System;
using Godot;

public class SmartSerialData {
    private readonly object _data;
    private readonly Action<object> _setter;
    private readonly Action _fallback;

    private SmartSerialData(object data, Action<object> setter, Action fallback) {
        _data = data;
        _setter = setter;
        _fallback = fallback;
    }

    public object GetData() => _data;
    public Action<object> GetSetter() => _setter;
    public Action GetFallback() => _fallback;

    public static SmartSerialData From(object data, Action<object> setter, Action fallback) {
        return new SmartSerialData(data, setter, fallback);
    }

    public static SmartSerialData FromInventory(IInventory inventory, Action fallbackOverride = null) {
        return From(inventory.Serialise(),
            v => {
                if (v is not string s) {
                    GD.PrintErr($"ERROR: FloodlightObject.GetSerialiseData() : Failed to deserialise inventory data. Expected string, got {v ?? "null"}");
                    return;
                }
                inventory.Deserialise(s);
            },
            () => {
                if (fallbackOverride == null) inventory.ClearContents();
                else fallbackOverride();
            });
    }
}