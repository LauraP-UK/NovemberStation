
using System;
using Godot;

public class ButtonElement : FormElement<Button> {

    public ButtonElement(Button element = null) {
        if (element != null) SetElement(element);
    }

    public void Build(string text, Vector2 size, Vector2 position) {
        Button element = new();
        element.Text = text;
        element.Size = size;
        element.Position = position;
        SetElement(element);
    }
    
    public void SetOnClick(Action<object[]> action) => AddAction(Signals.Button.PRESSED, action);
}