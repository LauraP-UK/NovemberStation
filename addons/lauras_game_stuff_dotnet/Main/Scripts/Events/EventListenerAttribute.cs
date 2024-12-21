
using System;

[AttributeUsage(AttributeTargets.Method)]
public class EventListenerAttribute : Attribute {
    public int Priority { get; }
    public EventListenerAttribute(int priority = PriorityLevels.NORMAL) {
        Priority = priority;
    }
}