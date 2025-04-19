using System;

[AttributeUsage(AttributeTargets.Field)]
public class SerialiseDataAttribute : Attribute {
    public enum SerialiseHandler {
        AUTO,
        INVENTORY
    }
    public string Key { get; }
    public string Setter { get; }
    public string Fallback { get; }
    public SerialiseHandler Handler { get; }
    public SerialiseDataAttribute(string key, string setter, string fallback, SerialiseHandler handler = SerialiseHandler.AUTO) {
        Key = key;
        Setter = setter;
        Fallback = fallback;
        Handler = handler;
    }

    public SerialiseDataAttribute(string key, string setterAndFallback, SerialiseHandler handler = SerialiseHandler.AUTO) {
        Key = key;
        Setter = setterAndFallback;
        Fallback = setterAndFallback;
        Handler = handler;
    }
}