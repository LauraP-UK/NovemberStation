using System;
using System.Collections.Generic;
using System.Linq;

public class SmartSet<T> : HashSet<T> {
    public void RemoveAll(IEnumerable<T> items) {
        if (items == null) throw new ArgumentNullException(nameof(items));
        foreach (T item in items) Remove(item);
    }
    
    public void RemoveAll(params T[] items) {
        if (items == null) throw new ArgumentNullException(nameof(items));
        RemoveAll((IEnumerable<T>)items);
    }
    
    public void RemoveWhere(Func<T, bool> predicate) {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        foreach (T item in this.Where(predicate).ToList()) Remove(item);
    }
}