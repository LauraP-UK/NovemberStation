
using System;
using System.Collections.Generic;
using Godot;

public class VBoxContainerLayout : LayoutElement<VBoxContainer> {
    
    private readonly List<IFormObject> _displayObjects = new();
    
    public VBoxContainerLayout(VBoxContainer container = null, Action<VBoxContainer> onReady = null) : base(container, onReady) { }
    public VBoxContainerLayout(string path, Action<VBoxContainer> onReady = null) : base(path, onReady) { }
    
    public void SetAlignment(BoxContainer.AlignmentMode value) => GetContainer().SetAlignment(value);
    public List<IFormObject> GetDisplayObjects() => _displayObjects;
    
    public void AddChild(IFormObject child) {
        _displayObjects.Add(child);
        GetContainer().AddChild(child.GetNode());
    }
}