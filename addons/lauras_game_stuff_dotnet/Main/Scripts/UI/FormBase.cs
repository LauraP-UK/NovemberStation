using System.Collections.Generic;
using Godot;

public abstract class FormBase : IFormObject {
    protected readonly Control _menu;
    protected ControlElement _menuElement;
    protected IFormObject _topLevelLayout;

    private readonly SignalNode _onReadyAction;
    protected bool _captureInput = true;

    protected FormBase(string formName, string formPath) {
        _menu = Loader.SafeInstantiate<Control>(formPath, true);
        _menu.Name = formName;
        _onReadyAction = new SignalNode(Node.SignalName.Ready, (_, _) => EventManager.RegisterListeners(this), this);
        GetMenu().AddChild(_onReadyAction);
    }

    public void AddToScene(CanvasLayer uiLayer) => uiLayer.AddChild(GetMenu());

    public void RemoveFromScene() => Destroy();

    protected abstract List<IFormObject> GetAllElements();
    protected List<T> GetElements<T>() where T : IFormObject {
        List<T> elements = new();
        foreach (IFormObject element in GetAllElements())
            if (element is T tElement) elements.Add(tElement);
        return elements;
    }

    protected abstract void OnDestroy();
    protected virtual void KeyboardBehaviour(Key key, bool isPressed) {}
    public virtual bool LockMovement() => true;

    public Control GetMenu() => _menu;
    public void Destroy() {
        if (!IsValid()) return;
        OnDestroy();

        if (this is IFocusable thisFocusable) thisFocusable.ReleaseFocus();
        
        EventManager.UnregisterListeners(this);
        
        foreach (IFormObject formElement in GetAllElements()) {
            formElement.Destroy();
            formElement.GetNode().QueueFree();
        }
        _onReadyAction.QueueFree();
        _menu.QueueFree();
    }
    
    protected T FindNode<T>(string nodePath) where T : Node => _menu.GetNode<T>(nodePath);
    
    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnKeyPress(KeyPressEvent ev, Key key) {
        if (GetTopLevelLayout().CaptureInput()) ev.Capture();
        KeyboardBehaviour(key, true);
    }

    [EventListener(PriorityLevels.TERMINUS)]
    protected void OnKeyRelease(KeyReleaseEvent ev, Key key) => KeyboardBehaviour(key, false);

    public IFormObject GetTopLevelLayout() {
        if (_topLevelLayout == null) return this;
        return Equals(_topLevelLayout) ? this : _topLevelLayout;
    }

    public void SetTopLevelLayout(IFormObject layout) => _topLevelLayout = layout;
    public void SetCaptureInput(bool value) => _captureInput = value;
    public bool CaptureInput() => GetTopLevelLayout().Equals(this) ? _captureInput : GetTopLevelLayout().CaptureInput();
    public Control GetNode() => _menu;
    protected bool IsValid() => GodotObject.IsInstanceValid(_menu) && !_menu.IsQueuedForDeletion();
}