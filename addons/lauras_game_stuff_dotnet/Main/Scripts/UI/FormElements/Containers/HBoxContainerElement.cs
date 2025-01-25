using System;
using Godot;

public class HBoxContainerElement : FormElement<HBoxContainer> {
    public HBoxContainerElement(HBoxContainer container = null, Action<HBoxContainer> onReady = null) : base(container, onReady) { }
    public HBoxContainerElement(string path, Action<HBoxContainer> onReady = null) : base(path, onReady) { }
}