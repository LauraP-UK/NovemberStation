using System;
using System.Collections.Generic;

public class EventManager {
    private static EventManager instance;

    private readonly Dictionary<Type, HashSet<Listener>> _listeners = new();

    public EventManager() {
        if (instance == null) instance = this;
        else throw new InvalidOperationException("ERROR: EventManager.<init> : Multiple EventManagers attempted to be initialised.");
        Scheduler.ScheduleRepeating(5000L, 5000L, _ => { CleanupListeners(); });
    }

    public static EventManager I() {
        if (instance == null) throw new InvalidOperationException("ERROR: EventManager.I() : EventManager has not been initialised, make sure to create it first!");
        return instance;
    }

    public void RegisterListener<TEvent, TContext>(Action<TEvent, TContext> callback, object owner = null) where TEvent : EventBase<TContext> {
        Type eventID = typeof(TEvent);

        if (!_listeners.ContainsKey(eventID)) _listeners[eventID] = new HashSet<Listener>();

        object actualOwner = owner ?? callback.Target;
        if (actualOwner == null) throw new ArgumentException("ERROR: EventManager.RegisterListener() : Owner cannot be null if callback has no target.");

        Listener toAdd = new(callback, actualOwner);

        if (_listeners[eventID].Contains(toAdd)) {
            Console.WriteLine($"WARN: EventManager.RegisterListener() : Listener of type {eventID.FullName} already registered with Owner {actualOwner}!");
            return;
        }

        _listeners[eventID].Add(toAdd);
    }

    public void UnregisterListener<TEvent, TContext>(Action<TEvent, object> callback) where TEvent : EventBase<TContext> {
        Type eventID = typeof(TEvent);

        if (_listeners.TryGetValue(eventID, out HashSet<Listener> list))
            list.RemoveWhere(listener => ReferenceEquals(listener.Callback, callback));
    }

    public void FireEvent<TEvent, TContext>(TEvent ev) where TEvent : EventBase<TContext> {
        Type eventID = typeof(TEvent);
        if (!_listeners.TryGetValue(eventID, out HashSet<Listener> listenerList)) return;

        TContext additionalContext = ev.GetAdditionalContext();

        HashSet<Listener> toRemove = new();
        foreach (Listener listener in new HashSet<Listener>(listenerList)) {
            if (listener.Owner != null && listener.Callback is Action<TEvent, TContext> action) {
                Console.WriteLine($"INFO: EventManager.FireEvent() :  Firing event of type {eventID} with {listenerList.Count} listeners.");
                action(ev, additionalContext);
            }
            else
                toRemove.Add(listener);
        }

        if (toRemove.Count != 0)
            foreach (Listener listener in toRemove)
                listenerList.Remove(listener);
    }

    private void CleanupListeners() {
        Console.WriteLine("INFO: EventManager.CleanupListeners() : Cleaning up listeners.");
        foreach (Type key in _listeners.Keys)
            _listeners[key].RemoveWhere(listener => listener.Owner == null);
    }
}