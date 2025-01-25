using System;
using System.Collections.Generic;
using Godot;

public class VBoxContainerElement : FormElement<VBoxContainer> {
    private readonly List<IFormObject> _displayObjects = new();
    public VBoxContainerElement(VBoxContainer container = null, Action<VBoxContainer> onReady = null) : base(container, onReady) { }
    public VBoxContainerElement(string path, Action<VBoxContainer> onReady = null) : base(path, onReady) { }
    public void SetAlignment(BoxContainer.AlignmentMode value) => GetElement().SetAlignment(value);
    public List<IFormObject> GetDisplayObjects() => _displayObjects;
    public void AddChild(IFormObject child) {
        _displayObjects.Add(child);
        GetElement().AddChild(child.GetNode());
    }
}