
using System;
using Godot;

public class ButtonElementBase<T> : FormElement<T> where T : BaseButton {
    
    public ButtonElementBase(T element = null) : base(element) { }
    public ButtonElementBase(string text, Action<T> initialiser = null) : base(text, initialiser) { }
    
    public void SetOnPressed(Action action) => AddAction(BaseButton.SignalName.Pressed, _ => action());
    public void SetOnButtonDown(Action action) => AddAction(BaseButton.SignalName.ButtonDown, _ => action());
    public void SetOnButtonUp(Action action) => AddAction(BaseButton.SignalName.ButtonUp, _ => action());
    public void SetOnToggled(Action<object[]> action) => AddAction(BaseButton.SignalName.Toggled, action);
}