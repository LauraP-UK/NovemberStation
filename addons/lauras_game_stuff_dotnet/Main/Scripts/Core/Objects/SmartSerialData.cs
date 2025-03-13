using System;

public class SmartSerialData {
    private object _data;
    private Action<object> _setter;
    private Action _fallback;
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
}