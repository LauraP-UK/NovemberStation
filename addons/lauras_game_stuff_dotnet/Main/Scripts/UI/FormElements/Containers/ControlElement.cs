using System;
using Godot;

public class ControlElement : FormElement<Control> {
    public ControlElement(Control container = null, Action<Control> onReady = null) : base(container, onReady) { }
    public ControlElement(string path, Action<Control> onReady = null) : base(path, onReady) { }
}