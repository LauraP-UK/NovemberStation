using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;

public static class Serialiser {
    private static readonly Cache<string, SmartDictionary<string, object>> _cache = new(() => new SmartDictionary<string, object>());

    public static string Serialise<T>(T obj) {
        return JsonSerializer.Serialize(
            obj, new JsonSerializerOptions {
                WriteIndented = true,
                IncludeFields = false
            }
        );
    }

    public static T Deserialise<T>(string json) => JsonSerializer.Deserialize<T>(json);

    public static T GetSpecificTag<T>(string dataTag, string json) {
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty(dataTag, out JsonElement element)) return default;

        object raw = ExtractData(element);
        if (raw is T tVal) return tVal;
        try {
            return (T)Convert.ChangeType(raw, typeof(T));
        } catch {
            GD.PrintErr($"WARN: Serialiser.GetSpecificData() : Failed to convert {raw} to {typeof(T)}");
        }

        return default;
    }

    public static T GetSpecificData<T>(string key, string json) {
        object result = _cache.GetFromCache(json).GetOrDefault(key, null);
        if (result != null) {
            if (result is T converted) return converted;
            try {
                return (T)Convert.ChangeType(result, typeof(T));
            } catch {
                GD.PrintErr($"WARN: Serialiser.GetSpecificData() : Failed to convert {result} to {typeof(T)}");
                return default;
            }
        }

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty(ObjectSaveData.DATA, out JsonElement dataElement)) return default;
        if (!dataElement.TryGetProperty(key, out JsonElement target)) return default;

        object raw = ExtractData(target);
        _cache.AddToCache(json, (cache) => {
            SmartDictionary<string,object> kVs = cache.GetFromCache(json);
            kVs.Add(key, raw);
            return kVs;
        });
        if (raw is T tVal) return tVal;

        try {
            return (T)Convert.ChangeType(raw, typeof(T));
        } catch {
            GD.PrintErr($"WARN: Serialiser.GetSpecificData() : Failed to convert {raw} to {typeof(T)}");
            return default;
        }
    }

    public static string ModifySpecificData<T>(string key, Func<T, T> transform, string json) {
        T current = GetSpecificData<T>(key, json);
        if (current != null) return SetSpecificData(key, transform(current), json);
        GD.PrintErr($"WARN: Serialiser.ModifySpecificData() : Could not get value for key '{key}' from JSON.");
        return json;
    }

    public static string SetSpecificData(string key, object value, string json) {
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty(ObjectSaveData.DATA, out JsonElement dataElement)) {
            GD.PrintErr($"ERROR: Serialiser.SetSpecificData() : '{ObjectSaveData.DATA}' field missing in JSON!");
            return json;
        }

        Dictionary<string, object> dataDict = new();
        foreach (JsonProperty prop in dataElement.EnumerateObject()) dataDict[prop.Name] = ExtractData(prop.Value);

        dataDict[key] = value;

        ObjectSaveData newObject = new() {
            MetaTag = root.GetProperty(ObjectSaveData.META_TAG).GetString(),
            TypeID = root.GetProperty(ObjectSaveData.TYPE_ID).GetString(),
            Data = dataDict
        };

        return Serialise(newObject);
    }

    public static object ExtractData(JsonElement elem) {
        return elem.ValueKind switch {
            JsonValueKind.Number => elem.TryGetInt64(out long l) ? l : elem.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => elem.GetString(),
            JsonValueKind.Object => elem,
            JsonValueKind.Array => elem,
            _ => null
        };
    }

    public class ObjectSaveData {
        public const string META_TAG = "MetaTag", TYPE_ID = "TypeID", DATA = "Data";
        public string MetaTag { get; set; }
        public string TypeID { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();

        public void SanitiseToObjects() {
            Dictionary<string, object> newData = new();
            foreach ((string key, object value) in Data) {
                if (value is JsonElement elem)
                    newData[key] = ExtractData(elem);
                else
                    newData[key] = value;
            }

            Data = newData;
        }
    }
}