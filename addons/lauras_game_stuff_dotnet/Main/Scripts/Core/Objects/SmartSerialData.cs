using System;
using Godot;

public class SmartSerialData {
    private readonly Func<object> _getter;
    private readonly Action<object> _setter;
    private readonly Action _fallback;
    private Func<object, object> _instanceGetter;
    private Action<object, object> _instanceSetter;
    private Action<object> _instanceFallback;

    private SmartSerialData(Func<object> getter, Action<object> setter, Action fallback) {
        _getter = getter;
        _setter = setter;
        _fallback = fallback;
    }

    public object GetData() {
        object value = _getter();
        if (value is Delegate)
            throw new Exception("ERROR: SmartSerialData.GetData() returned a delegate! This is not allowed.");
        return value;
    }

    public Action<object> GetSetter() => _setter;
    public Action GetFallback() => _fallback;

    public SmartSerialData WithInstanceGetter(Func<object, object> getter) {
        _instanceGetter = getter;
        return this;
    }

    public object InvokeInstanceGetter(object targetInstance) => _instanceGetter?.Invoke(targetInstance);

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

    public static SmartSerialData From(Func<object> getter, Action<object> setter, Action fallback) {
        return new SmartSerialData(getter, setter, fallback);
    }

    public static SmartSerialData FromInventory() {
        return From(
                () => throw new InvalidOperationException("Use instance-aware getter"),
                v => throw new InvalidOperationException("Use instance-aware setter"),
                () => throw new InvalidOperationException("Use instance-aware fallback")
            ).WithInstanceSetter(InventorySerialiseHandlers.SetInventory)
            .WithInstanceFallback(InventorySerialiseHandlers.FallbackInventory)
            .WithInstanceGetter(InventorySerialiseHandlers.GetInventory);
    }
}