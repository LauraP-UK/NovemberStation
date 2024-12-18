
using System;
using Godot;

public abstract class EventBase<T> {
    public abstract T GetAdditionalContext();

    public virtual void Fire() {
        GD.Print($"INFO: EventBase.Fire() : Firing event of type {GetType().FullName}");
        EventManager.I().FireEvent<EventBase<T>, T>(this);
    }
}
