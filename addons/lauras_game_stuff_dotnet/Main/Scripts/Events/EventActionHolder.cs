
using System;

public class EventActionHolder : IEquatable<EventActionHolder> {
    public Delegate Callback { get; }
    public object Owner { get; }

    public EventActionHolder(Delegate callback, object owner) {
        Callback = callback;
        Owner = owner;
    }

    public bool Equals(EventActionHolder other) {
        return ReferenceEquals(Callback, other?.Callback) && ReferenceEquals(Owner, other?.Owner);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Callback, Owner);
    }
}