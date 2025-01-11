using System;
using Godot;

public class ControlLayout : LayoutElement<Control> {
    
    public ControlLayout(Control container = null, Action<Control> onReady = null) : base(container, onReady) { }
    public ControlLayout(string path, Action<Control> onReady = null) : base(path, onReady) { }
    
}