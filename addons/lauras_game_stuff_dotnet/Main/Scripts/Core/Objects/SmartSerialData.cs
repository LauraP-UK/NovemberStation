using System;
using Godot;

public class SmartSerialData {
    private readonly object _data;
    private readonly Action<object> _setter;
    private readonly Action _fallback;
    private Action<object, object> _instanceSetter;
    private Action<object> _instanceFallback;

    private SmartSerialData(object data, Action<object> setter, Action fallback) {
        _data = data;
        _setter = setter;
        _fallback = fallback;
    }

    public object GetData() => _data;
    public Action<object> GetSetter() => _setter;
    public Action GetFallback() => _fallback;
    public override string ToString() => $"[ Data : {_data} , Setter : {_setter} , Fallback : {_fallback} ]";

    public SmartSerialData WithInstanceSetter(Action<object, object> setter) {
        _instanceSetter = setter;
        return this;
    }
    public void InvokeInstanceSetter(object targetInstance, object value) => _instanceSetter?.Invoke(targetInstance, value);

    public SmartSerialData WithInstanceFallback(Action<object> fallback) {
        _instanceFallback = fallback;
        return this;
    }
    public void InvokeInstanceFallback(object targetInstance) => _instanceFallback?.Invoke(targetInstance);

    public static SmartSerialData From(object data, Action<object> setter, Action fallback) {
        return new SmartSerialData(data, setter, fallback);
    }

    public static SmartSerialData FromInventory(IInventory inventory, Action fallbackOverride = null) {
        string json = inventory.Serialise();
        return From(json,
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