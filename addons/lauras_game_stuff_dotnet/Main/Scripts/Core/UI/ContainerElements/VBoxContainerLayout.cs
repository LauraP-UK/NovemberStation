
using System;
using Godot;

public class VBoxContainerLayout : LayoutElement<VBoxContainer> {
    
    public VBoxContainerLayout(VBoxContainer container = null, Action<VBoxContainer> onReady = null) : base(container, onReady) { }
    public VBoxContainerLayout(string path, Action<VBoxContainer> onReady = null) : base(path, onReady) { }
    
}