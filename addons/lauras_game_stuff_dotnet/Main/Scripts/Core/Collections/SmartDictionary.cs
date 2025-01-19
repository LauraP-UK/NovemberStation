using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

public class SmartDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue> {
    
    public SmartDictionary() { }
    public SmartDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
    
    public new void Add(TKey key, TValue value) {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (ContainsKey(key))
            this[key] = value;
        else
            AddOrUpdate(key, value, (_, _) => value);
    }

    public void Merge(IDictionary<TKey, TValue> other, bool overwrite = true) {
        if (other == null) throw new ArgumentNullException(nameof(other));
        foreach (KeyValuePair<TKey, TValue> pair in other)
            if (overwrite || !ContainsKey(pair.Key)) this[pair.Key] = pair.Value;
    }

    public void Merge(IDictionary<TKey, TValue> other, Func<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>, TValue> conflictResolver) {
        if (other == null) throw new ArgumentNullException(nameof(other));
        if (conflictResolver == null) throw new ArgumentNullException(nameof(conflictResolver));

        foreach (KeyValuePair<TKey, TValue> pair in other) {
            if (ContainsKey(pair.Key)) {
                TValue resolvedValue = conflictResolver(new KeyValuePair<TKey, TValue>(pair.Key, this[pair.Key]), pair);
                this[pair.Key] = resolvedValue;
            }
            else Add(pair.Key, pair.Value);
        }
    }

    public TValue GetOrCompute(TKey key, Func<TValue> valueFactory) {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));
        if (TryGetValue(key, out TValue value)) return value;
        TValue newValue = valueFactory();
        Add(key, newValue);
        return newValue;
    }

    public TValue GetOrDefault(TKey key, TValue defaultValue) {
        if (key == null) throw new ArgumentNullException(nameof(key));
        return TryGetValue(key, out TValue value) ? value : defaultValue;
    }
    
    public SmartDictionary<TKey, TValue> Clone() => new(this);
    
    public IDictionary<TKey, TValue> ClearAndReturn() {
        IDictionary<TKey, TValue> copy = new SmartDictionary<TKey, TValue>(this);
        Clear();
        return copy;
    }

    public void RemoveAll(IEnumerable<TKey> keys) {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        foreach (TKey key in keys) Remove(key);
    }

    public void RemoveAll(params TKey[] keys) {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        RemoveAll((IEnumerable<TKey>)keys);
    }

    public void RemoveWhere(Func<KeyValuePair<TKey, TValue>, bool> predicate) {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        foreach (KeyValuePair<TKey, TValue> pair in this.Where(predicate).ToList()) Remove(pair.Key);
    }
    
    public bool Remove(TKey key) => TryRemove(key, out TValue _);
}