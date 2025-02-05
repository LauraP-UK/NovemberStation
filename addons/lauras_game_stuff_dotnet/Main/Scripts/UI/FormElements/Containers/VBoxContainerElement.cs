using System;
using System.Collections.Generic;
using Godot;

public class VBoxContainerElement : FormElement<VBoxContainer> {
    private readonly List<IFormObject> _displayObjects = new();
    private bool _uniquesOnly;
    public VBoxContainerElement(VBoxContainer container = null, Action<VBoxContainer> onReady = null) : base(container, onReady) { }
    public VBoxContainerElement(string path, Action<VBoxContainer> onReady = null) : base(path, onReady) { }
    public void SetAlignment(BoxContainer.AlignmentMode value) => GetElement().SetAlignment(value);
    public void SetUniquesOnly(bool value) => _uniquesOnly = value;
    public List<IFormObject> GetDisplayObjects() => _displayObjects;
    public bool IsEmpty() => _displayObjects.Count == 0;
    public int GetChildCount() => _displayObjects.Count;
    public void AddChild(IFormObject child) {
        if (_uniquesOnly && _displayObjects.Contains(child)) {
            child.GetNode().QueueFree();
            return;
        }
        _displayObjects.Add(child);
        GetElement().AddChild(child.GetNode());
    }
    
    public void SetChildren<T>(List<T> children) where T : IFormObject {
        if (ArrayUtils.ExactMatch(children, _displayObjects)) return;
        ClearChildren();
        foreach (T child in children) AddChild(child);
    }
    
    public void ClearChildren() {
        foreach (IFormObject displayObject in _displayObjects) {
            displayObject.Destroy();
            displayObject.GetNode().QueueFree();
        }
        _displayObjects.Clear();
    }
}