

using System.Collections.Generic;
using System.Text.Json;

public static class Serialiser {
    public static string Serialise<T>(T obj) => JsonSerializer.Serialize(obj);
    public static T Deserialise<T>(string data) => JsonSerializer.Deserialize<T>(data);
    
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
        public string MetaTag { get; set; }
        public string ScenePath { get; set; }
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