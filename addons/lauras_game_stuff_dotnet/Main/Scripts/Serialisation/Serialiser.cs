

using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

public static class Serialiser {
    public static string Serialise<T>(T obj) => JsonSerializer.Serialize(obj);
    public static T Deserialise<T>(string json) => JsonSerializer.Deserialize<T>(json);
    public static T GetSpecificData<T>(string dataTag, string json) {
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty(dataTag, out JsonElement element)) return default;
        
        object raw = ExtractData(element);
        if (raw is T tVal)
            return tVal;
        try {
            return (T)Convert.ChangeType(raw, typeof(T));
        } catch {
            GD.PrintErr($"WARN: GetSpecificData: Failed to convert {raw} to {typeof(T)}");
        }
        return default;
    }
    public static object ExtractData(JsonElement elem) {
        return elem.ValueKind switch {
            JsonValueKind.Number => elem.TryGetInt64(out long l) ? l : elem.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => elem.GetString(),
            _ => null
        };
    }
    public class ObjectSaveData {
        public const string META_TAG = "MetaTag", TYPE_ID = "TypeID", DATA = "Data";
        public string MetaTag { get; set; }
        public string TypeID { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
        public void SanitiseToObjects() {
            Dictionary<string,object> newData = new();
            foreach ((string key, object value) in Data) {
                if (value is JsonElement elem) newData[key] = ExtractData(elem);
                else newData[key] = value;
            }
            Data = newData;
        }
    }
}