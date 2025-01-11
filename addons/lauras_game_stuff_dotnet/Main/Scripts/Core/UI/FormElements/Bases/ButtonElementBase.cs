
using System;
using Godot;

public class ButtonElementBase<T> : FormElement<T> where T : BaseButton {
    protected ButtonElementBase(T element = null) : base(element) { }
    protected ButtonElementBase(string text, Action<T> initialiser = null) : base(text, initialiser) { }
    
    public void OnPressed(Action<IFormObject> action) => AddAction(BaseButton.SignalName.Pressed, action);
    public void OnButtonDown(Action<IFormObject> action) => AddAction(BaseButton.SignalName.ButtonDown, action);
    public void OnButtonUp(Action<IFormObject> action) => AddAction(BaseButton.SignalName.ButtonUp, action);
    public void OnToggled(Action<IFormObject, object[]> action) => AddAction(BaseButton.SignalName.Toggled, action);
}