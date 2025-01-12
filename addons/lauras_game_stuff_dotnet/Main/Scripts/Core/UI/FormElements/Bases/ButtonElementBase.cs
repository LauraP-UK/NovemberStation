
using System;
using Godot;

public class ButtonElementBase<T> : FormElement<T> where T : BaseButton {
    protected ButtonElementBase(T element = null, Action<T> onReady = null) : base(element, onReady) { }
    protected ButtonElementBase(string text, Action<T> onReady = null) : base(text, onReady) { }
    
    public void OnPressed(Action<IFormObject> action) => AddAction(BaseButton.SignalName.Pressed, action);
    public void OnButtonDown(Action<IFormObject> action) => AddAction(BaseButton.SignalName.ButtonDown, action);
    public void OnButtonUp(Action<IFormObject> action) => AddAction(BaseButton.SignalName.ButtonUp, action);
    public void OnToggled(Action<IFormObject, object[]> action) => AddAction(BaseButton.SignalName.Toggled, action);
    
    public void ForcePressed() => GetElement().EmitSignal(BaseButton.SignalName.Pressed);
    public void ForceButtonDown() => GetElement().EmitSignal(BaseButton.SignalName.ButtonDown);
    public void ForceButtonUp() => GetElement().EmitSignal(BaseButton.SignalName.ButtonUp);
    public void ForceToggled(bool pressed) => GetElement().EmitSignal(BaseButton.SignalName.Toggled, pressed);
    
}