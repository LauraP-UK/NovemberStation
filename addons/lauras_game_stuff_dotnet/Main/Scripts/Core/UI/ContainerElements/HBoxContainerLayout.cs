
using System;
using Godot;

public class HBoxContainerLayout : LayoutElement<HBoxContainer> {
    
    public HBoxContainerLayout(HBoxContainer container = null, Action<HBoxContainer> onReady = null) : base(container, onReady) { }
    public HBoxContainerLayout(string path, Action<HBoxContainer> onReady = null) : base(path, onReady) { }
    
}