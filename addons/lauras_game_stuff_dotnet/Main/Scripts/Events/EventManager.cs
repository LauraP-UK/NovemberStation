using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Environment = System.Environment;

public class EventManager {
    private static EventManager instance;

    private readonly SmartDictionary<Type, SmartSet<EventActionHolder>> _listeners = new();

    public EventManager() {
        if (instance == null) instance = this;
        else throw new InvalidOperationException("ERROR: EventManager.<init> : Multiple EventManagers attempted to be initialised.");
        Scheduler.ScheduleRepeating(5000L, 5000L, _ => CleanupListeners());
    }
    
    public static void HookWindowResize(Viewport viewport) {
        viewport.Connect(Viewport.SignalName.SizeChanged, Callable.From(() => {
            WindowResizeEvent ev = new();
            ev.SetSize(viewport.GetVisibleRect().Size);
            ev.Fire();
        }));
    }
    
    public static EventManager I() {
        if (instance == null) throw new InvalidOperationException("ERROR: EventManager.I() : EventManager has not been initialised, make sure to create it first!");
        return instance;
    }

    public static void RegisterListeners(object target) {
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        
        GD.Print($"-----  ({target})  :  " + Environment.StackTrace);
        
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
    
    public static void UnregisterListeners(object owner) => I().UnregisterByOwner(owner);

    public void RegisterListener(Type eventType, Delegate callback, int priority, object owner = null) {
        if (!_listeners.ContainsKey(eventType)) _listeners[eventType] = new SmartSet<EventActionHolder>();

        object actualOwner = owner ?? callback.Target;
        if (actualOwner == null) throw new ArgumentException("ERROR: EventManager.RegisterListener() : Owner cannot be null if callback has no target.");

        EventActionHolder toAdd = new(callback, actualOwner, priority);

        //GD.Print($"INFO: Checking if _listeners contains key {eventType.FullName}  |  EventActionHolder {toAdd.GetHashCode()}");
        
        if (_listeners[eventType].Contains(toAdd)) {
            GD.Print($"WARN: Listener of type {eventType.FullName} already registered with Owner {actualOwner}!");
            return;
        }
        
        //GD.Print($"INFO: Registering listener for event {eventType.FullName} with Owner {actualOwner} ({toAdd.GetHashCode()}).");
        
        _listeners[eventType].Add(toAdd);
    }
    
    public void RegisterListener<TEvent, TContext>(Action<TEvent, TContext> callback, int priority, object owner = null) where TEvent : EventBase<TContext> {
        RegisterListener(typeof(TEvent), callback, priority, owner);
    }

    public void UnregisterByOwner(object owner) {
        //GD.Print($"INFO: Unregistering listeners for Owner {owner}. There are currently {_listeners.Count} event types registered.");
        int removed = 0;
        foreach (Type key in _listeners.Keys) {
            //GD.Print($"INFO: Checking {_listeners[key].Count} {key.FullName} listeners...");
            removed += _listeners[key].RemoveWhere(listener => {
                //GD.Print($"INFO: Checking listener for event {key.FullName} with Owner {listener.Owner} ({listener.Owner.GetHashCode()}) against {owner} ({owner.GetHashCode()}).");
                bool toRemove = ReferenceEquals(listener.Owner, owner) || listener.Owner.Equals(owner);
                //if (toRemove) GD.Print($"INFO: Unregistered listener for event {key.FullName} with Owner {owner} ({owner.GetHashCode()}).");
                return toRemove;
            });
        }
        //GD.Print($"INFO: Unregistered {removed} total listeners for Owner {owner}.");
    }
    public void UnregisterListener<TEvent, TContext>(Action<TEvent, object> callback) where TEvent : EventBase<TContext> {
        Type eventID = typeof(TEvent);

        if (_listeners.TryGetValue(eventID, out SmartSet<EventActionHolder> list))
            list.RemoveWhere(listener => ReferenceEquals(listener.Callback, callback));
    }

    public void FireEvent<TContext>(EventBase<TContext> ev) {
        Type eventID = ev.GetType();
        if (!_listeners.TryGetValue(eventID, out SmartSet<EventActionHolder> listenerList))
            return;

        TContext additionalContext = ev.GetAdditionalContext();
        
        List<EventActionHolder> sortedListeners = listenerList
            .OrderByDescending(l => l.Priority)
            .ThenBy(l => l.RegisteredAt)
            .ToList();

        HashSet<EventActionHolder> toRemove = new();
        foreach (EventActionHolder listener in sortedListeners) {
            if (listener.Owner != null && listener.Callback is Delegate callback) {
                try {
                    callback.DynamicInvoke(ev, additionalContext);
                    if (ev is ICancellable cancelable && cancelable.IsCanceled()) {
                        ev.PrintLog();
                        break;
                    }
                } catch (Exception e) {
                    GD.PrintErr($"ERROR: EventManager.FireEvent : Exception thrown while invoking listener for event {eventID.FullName} : {e}");
                    GD.PrintErr($"Listener: {listener.Owner?.GetType().Name ?? "Unknown"} | Callback: {callback.Method.Name}");
                    GD.PrintErr($"Exception: {e.Message}\n{e.StackTrace}");
                }
            } else toRemove.Add(listener);
        }

        listenerList.RemoveAll(toRemove);
    }

    private void CleanupListeners() {
        foreach (Type key in _listeners.Keys)
            _listeners[key].RemoveWhere(listener => listener.Owner == null);
    }
}