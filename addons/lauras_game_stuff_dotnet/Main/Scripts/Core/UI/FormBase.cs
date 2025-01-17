
using System.Collections.Generic;
using Godot;

public abstract class FormBase : Listener, IFormObject {
    
    protected readonly Control _menu;
    protected ControlLayout _menuLayout;

    protected FormBase(string formName, string formPath) {
        _menu = Loader.SafeInstantiate<Control>(formPath, true);
        _menu.Name = formName;
    }

    protected abstract List<IFormObject> getAllElements();
    protected List<T> GetElements<T>() where T : IFormObject {
        List<T> elements = new();
        foreach (IFormObject element in getAllElements())
            if (element is T tElement) elements.Add(tElement);
        return elements;
    }
    
    protected virtual void OnDestroy() { }
    protected virtual void KeyboardBehaviour(Key key, bool isPressed) {}

    public Control GetMenu() => _menu;
    public void Destroy() {
        if (!IsValid() || _menu.IsQueuedForDeletion()) return;
        OnDestroy();

        EventManager eventManager = EventManager.I();
        _menu.QueueFree();
        UnregisterListeners();
        foreach (IFormObject formElement in getAllElements()) {
            eventManager.UnregisterByOwner(formElement);
            formElement.GetNode().QueueFree();
        }
    }
    
    protected T FindNode<T>(string nodePath) where T : Node => _menu.GetNode<T>(nodePath);
    
    [EventListener]
    protected void OnKeyPress(KeyPressEvent ev, Key key) => KeyboardBehaviour(key, true);
    
    [EventListener]
    protected void OnKeyRelease(KeyReleaseEvent ev, Key key) => KeyboardBehaviour(key, false);

    public void UnregisterListeners() => EventManager.I().UnregisterByOwner(this);

    public ILayoutElement GetTopLevelLayout() => _menuLayout;
    public void SetTopLevelLayout(ILayoutElement layout) {}
    public Control GetNode() => _menu;
    protected bool IsValid() => GodotObject.IsInstanceValid(_menu) && !_menu.IsQueuedForDeletion();
}