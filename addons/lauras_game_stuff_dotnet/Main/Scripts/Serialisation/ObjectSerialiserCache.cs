using Godot;

public static class ObjectSerialiserCache {
    private static readonly SmartDictionary<string, (ulong lastAccessed, SmartDictionary<string, object> kVs)> _cache = new(); // <Object JSON, (time last accessed, <Tag, Value>)>
    private static readonly SchedulerTask _clearTask;

    private const ulong CACHE_EXPIRY_TIME = 30000; // 30 seconds

    static ObjectSerialiserCache() {
        _clearTask = Scheduler.ScheduleRepeating(
            1000L,
            1000L,
            _ => _cache.RemoveWhere(e => Time.GetTicksMsec() - e.Value.lastAccessed > CACHE_EXPIRY_TIME)
        );
    }

    public static void AddToCache(string objectJson, string tag, object value) {
        (ulong _, SmartDictionary<string, object> kVs) = GetOrCompute(objectJson);
        kVs.Add(tag, value);
        _cache.Add(objectJson, (Time.GetTicksMsec(), kVs));
    }

    public static object GetFromCache(string objectJson, string tag) {
        (ulong _, SmartDictionary<string, object> kVs) = GetOrCompute(objectJson);
        object value = kVs.GetOrDefault(tag, null);
        if (value == null) return null;
        _cache.Add(objectJson, (Time.GetTicksMsec(), kVs));
        return value;
    }

    private static (ulong lastAccessed, SmartDictionary<string, object> kVs) GetOrCompute(string objectJson) =>
        _cache.GetOrCompute(objectJson, () => (Time.GetTicksMsec(), new SmartDictionary<string, object>()));
}