
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class EventBase<TContext> {
    private readonly List<LogItem> _log = new();
    public abstract TContext GetAdditionalContext();
    
    public void Fire() => EventManager.I().FireEvent(this);
    
    public string GetLog() => _log.Aggregate("", (current, item) => current + $"{item._timestamp} - {item._from} ({item._priority}): {item._message}\n");
    public void PrintLog() => GD.Print(GetLog());
    
    public void Log(string from, string message, int priority) {
        _log.Add(new LogItem {_from = from,_message = message,_priority = priority,_timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()});
    }
    
    private class LogItem {public string _from; public string _message; public int _priority; public long _timestamp;}
}
