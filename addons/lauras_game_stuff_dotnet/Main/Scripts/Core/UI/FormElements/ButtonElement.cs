
using System;
using Godot;

public class ButtonElement : ButtonElementBase<Button> {
    public ButtonElement(Button element = null) : base(element) {}
    public ButtonElement(string text, Action<Button> initialiser = null) : base(text, initialiser) {}
}