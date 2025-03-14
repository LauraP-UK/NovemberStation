using System;
using System.Collections.Generic;
using System.Linq;

public class SmartSet<T> : HashSet<T> {
    public SmartSet() { }
    public SmartSet(IEnumerable<T> items) : base(items) { }
    
    public SmartSet<T> Clone() => new(this);

    public IEnumerable<T> ClearAndReturn() {
        IEnumerable<T> items = this.ToList();
        Clear();
        return items;
    }
    
    public void RemoveAll(IEnumerable<T> items) {
        if (items == null) throw new ArgumentNullException(nameof(items));
        foreach (T item in items) Remove(item);
    }
    
    public void RemoveAll(params T[] items) {
        if (items == null) throw new ArgumentNullException(nameof(items));
        RemoveAll((IEnumerable<T>)items);
    }
    
    public void ForEach(Action<T> action) {
        if (action == null) return;
        foreach (T item in this) action(item);
    }
}