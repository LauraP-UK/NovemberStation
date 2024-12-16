using System.Collections.Generic;

public class AutoDictionary<TKey, TValue> : Dictionary<TKey, TValue> {

    public new void Add(TKey key, TValue value) {
        if (ContainsKey(key))
            this[key] = value;
        else
            base.Add(key, value);
    }
}