using System;
using Godot;

public class Cache<TKey, TValue> {
    private readonly SmartDictionary<TKey, (ulong lastAccessed, TValue)> _cache = new(); // <Key, (time last accessed, Value)>
    private readonly SchedulerTask _clearTask;

    private readonly long _cacheExpiryTime;
    private readonly Func<TValue> _valueFactory;

    public Cache(Func<TValue> valueFactory, long expireAfter = 30000) {
        _valueFactory = valueFactory;
        _cacheExpiryTime = expireAfter;
        if (expireAfter <= 0) return;
        _clearTask = Scheduler.ScheduleRepeating(
            1000L,
            1000L,
            _ => _cache.RemoveWhere(e => (long)Time.GetTicksMsec() - (long)e.Value.lastAccessed > _cacheExpiryTime)
        );
    }
    public void AddToCache(TKey key, Func<Cache<TKey, TValue>, TValue> addAction) =>
        _cache.Add(key, (Time.GetTicksMsec(), addAction(this)));
    
    public void AddToCache(TKey key, TValue value) => AddToCache(key, _ => value);

    public TValue GetFromCache(TKey key, bool computeIfMissing = true) {
        (_, TValue value) = computeIfMissing ? GetOrCompute(key) : _cache.GetOrDefault(key, (0, default));
        if (value == null) return default;
        _cache.Add(key, (Time.GetTicksMsec(), value));
        return value;
    }

    private (ulong lastAccessed, TValue value) GetOrCompute(TKey key) =>
        _cache.GetOrCompute(key, () => (Time.GetTicksMsec(), _valueFactory()));
    
    public void ClearCache() => _cache.Clear();
}