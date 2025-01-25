using System;
using Godot;

public class LabelElement : FormElement<Label> {
    public LabelElement(Label element = null, Action<Label> onReady = null) : base(element, onReady) {}
    public LabelElement(string text, Action<Label> onReady = null) : base(text, onReady) {}
}