using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

public class EventManager {
    private static EventManager instance;

    private readonly Dictionary<Type, HashSet<EventActionHolder>> _listeners = new();

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
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (MethodInfo method in methods) {
            EventListenerAttribute attribute = method.GetCustomAttribute<EventListenerAttribute>();
            if (attribute == null) continue;

            int priority = attribute.Priority;
            
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 2) continue; // Expecting 2 parameters: event instance and optional context

            Type eventType = parameters[0].ParameterType;
            Type contextType = parameters[1].ParameterType;

            Delegate callback = method.CreateDelegate(typeof(Action<,>).MakeGenericType(eventType, contextType), target);
            I().RegisterListener(eventType, callback, priority, target);
        }
    }

    public void RegisterListener(Type eventType, Delegate callback, int priority, object owner = null) {
        if (!_listeners.ContainsKey(eventType)) _listeners[eventType] = new HashSet<EventActionHolder>();

        object actualOwner = owner ?? callback.Target;
        if (actualOwner == null) throw new ArgumentException("Owner cannot be null if callback has no target.");

        EventActionHolder toAdd = new(callback, actualOwner, priority);

        if (_listeners[eventType].Contains(toAdd)) {
            GD.Print($"WARN: Listener of type {eventType.FullName} already registered with Owner {actualOwner}!");
            return;
        }

        _listeners[eventType].Add(toAdd);
    }
    
    public void RegisterListener<TEvent, TContext>(Action<TEvent, TContext> callback, int priority, object owner = null) where TEvent : EventBase<TContext> {
        RegisterListener(typeof(TEvent), callback, priority, owner);
    }

    public void UnregisterByOwner(object owner) {
        foreach (Type key in _listeners.Keys)
            _listeners[key].RemoveWhere(listener => ReferenceEquals(listener.Owner, owner));
    }
    public void UnregisterListener<TEvent, TContext>(Action<TEvent, object> callback) where TEvent : EventBase<TContext> {
        Type eventID = typeof(TEvent);

        if (_listeners.TryGetValue(eventID, out HashSet<EventActionHolder> list))
            list.RemoveWhere(listener => ReferenceEquals(listener.Callback, callback));
    }

    public void FireEvent<TContext>(EventBase<TContext> ev) {
        Type eventID = ev.GetType();
        if (!_listeners.TryGetValue(eventID, out HashSet<EventActionHolder> listenerList))
            return;

        TContext additionalContext = ev.GetAdditionalContext();
        
        List<EventActionHolder> sortedListeners = listenerList
            .OrderByDescending(l => l.Priority)
            .ThenBy(l => l.RegisteredAt)
            .ToList();

        HashSet<EventActionHolder> toRemove = new();
        foreach (EventActionHolder listener in sortedListeners) {
            if (listener.Owner != null && listener.Callback is Delegate callback) {
                callback.DynamicInvoke(ev, additionalContext);
                if (ev is ICancellable cancelable && cancelable.IsCanceled()) break;
            } else toRemove.Add(listener);
        }

        if (toRemove.Count != 0)
            foreach (EventActionHolder listener in toRemove)
                listenerList.Remove(listener);
    }

    private void CleanupListeners() {
        foreach (Type key in _listeners.Keys)
            _listeners[key].RemoveWhere(listener => listener.Owner == null);
    }
}