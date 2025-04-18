
using System;

[AttributeUsage(AttributeTargets.Interface)]
public class ObjectActionAttribute : Attribute {
    public string Name { get; }
    public int Index { get; }
    
    public ObjectActionAttribute(string name, int index) {
        Name = name;
        Index = index;
    }
}