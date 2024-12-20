using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

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

    public static void RegisterListeners(object target) {
        // Get all methods in the target object
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (MethodInfo method in methods) {
            // Check if the method has the EventListener attribute
            if (method.GetCustomAttribute<EventListenerAttribute>() == null) continue;

            // Ensure the method signature matches (e.g., parameters)
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 2) continue; // Expecting 2 parameters: event and context

            Type eventType = parameters[0].ParameterType;
            Type contextType = parameters[1].ParameterType;

            // Register the method as a listener with the EventManager
            Delegate callback = method.CreateDelegate(typeof(Action<,>).MakeGenericType(eventType, contextType), target);
            I().RegisterListener(eventType, callback, target);
        }
    }

    public void RegisterListener(Type eventType, Delegate callback, object owner = null) {
        if (!_listeners.ContainsKey(eventType)) _listeners[eventType] = new HashSet<Listener>();

        object actualOwner = owner ?? callback.Target;
        if (actualOwner == null) throw new ArgumentException("Owner cannot be null if callback has no target.");

        Listener toAdd = new(callback, actualOwner);

        if (_listeners[eventType].Contains(toAdd)) {
            GD.Print($"WARN: Listener of type {eventType.FullName} already registered with Owner {actualOwner}!");
            return;
        }

        _listeners[eventType].Add(toAdd);
    }
    
    public void RegisterListener<TEvent, TContext>(Action<TEvent, TContext> callback, object owner = null) where TEvent : EventBase<TContext> {
        RegisterListener(typeof(TEvent), callback, owner);
    }

    public void UnregisterListener<TEvent, TContext>(Action<TEvent, object> callback) where TEvent : EventBase<TContext> {
        Type eventID = typeof(TEvent);

        if (_listeners.TryGetValue(eventID, out HashSet<Listener> list))
            list.RemoveWhere(listener => ReferenceEquals(listener.Callback, callback));
    }

    public void FireEvent<TContext>(EventBase<TContext> ev) {
        Type eventID = ev.GetType();
        if (!_listeners.TryGetValue(eventID, out HashSet<Listener> listenerList))
            return;

        TContext additionalContext = ev.GetAdditionalContext();

        HashSet<Listener> toRemove = new();
        foreach (Listener listener in new HashSet<Listener>(listenerList)) {
            if (listener.Owner != null && listener.Callback is Delegate callback)
                callback.DynamicInvoke(ev, additionalContext);
            else
                toRemove.Add(listener);
        }

        if (toRemove.Count != 0)
            foreach (Listener listener in toRemove)
                listenerList.Remove(listener);
    }

    private void CleanupListeners() {
        foreach (Type key in _listeners.Keys)
            _listeners[key].RemoveWhere(listener => listener.Owner == null);
    }
}