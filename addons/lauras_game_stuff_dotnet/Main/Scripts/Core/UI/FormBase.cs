
using System.Collections.Generic;
using Godot;

public abstract class FormBase : Listener {
    
    protected readonly Control _menu;
    protected ControlLayout _menuLayout;

    protected FormBase(string formName, string formPath) {
        _menu = Loader.SafeInstantiate<Control>(formPath, true);
        _menu.Name = formName;
    }

    protected abstract List<IFormElement> GetElements();
    protected virtual void OnDestroy() { }
    protected virtual void KeyboardInput(Key key) {}

    public Control GetMenu() => _menu;
    public void Destroy() {
        EventManager eventManager = EventManager.I();
        _menu.QueueFree();
        eventManager.UnregisterByOwner(this);
        foreach (IFormElement formElement in GetElements()) eventManager.UnregisterByOwner(formElement);
    }
    
    [EventListener]
    protected void OnKeyPress(KeyPressEvent ev, Key key) => KeyboardInput(key);
}