
using System;

public class EventActionHolder : IEquatable<EventActionHolder> {
    public Delegate Callback { get; }
    public object Owner { get; }
    public int Priority { get; }
    public long RegisteredAt { get; }

    public EventActionHolder(Delegate callback, object owner, int priority) {
        Callback = callback;
        Owner = owner;
        Priority = priority;
        RegisteredAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public bool Equals(EventActionHolder other) {
        return ReferenceEquals(Callback, other?.Callback) && ReferenceEquals(Owner, other?.Owner);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Callback, Owner);
    }
}