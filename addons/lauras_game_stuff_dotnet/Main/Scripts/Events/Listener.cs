
using System;

public class Listener : IEquatable<Listener> {
    public Delegate Callback { get; }
    public object Owner { get; }

    public Listener(Delegate callback, object owner) {
        Callback = callback;
        Owner = owner;
    }

    public bool Equals(Listener other) {
        return ReferenceEquals(Callback, other?.Callback) && ReferenceEquals(Owner, other?.Owner);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Callback, Owner);
    }
}