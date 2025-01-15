
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
    protected virtual void KeyboardBehaviour(Key key) {}

    public Control GetMenu() => _menu;
    public void Destroy() {
        OnDestroy();
        if (_menu.IsQueuedForDeletion()) return;
        
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
    protected void OnKeyPress(KeyPressEvent ev, Key key) {
        GD.Print($"I am {GetType()} and I am handling a key press event. The key is {key}");
        KeyboardBehaviour(key);
    }

    public void UnregisterListeners() => EventManager.I().UnregisterByOwner(this);

    public ILayoutElement GetTopLevelLayout() => _menuLayout;
    public void SetTopLevelLayout(ILayoutElement layout) {}
    public Control GetNode() => _menu;
}