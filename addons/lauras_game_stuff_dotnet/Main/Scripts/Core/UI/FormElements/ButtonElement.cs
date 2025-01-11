
using System;
using Godot;

public class ButtonElement : ButtonElementBase<Button> {
    public ButtonElement(Button element = null, Action<Button> onReady = null) : base(element, onReady) {}
    public ButtonElement(string text, Action<Button> onReady = null) : base(text, onReady) {}
}