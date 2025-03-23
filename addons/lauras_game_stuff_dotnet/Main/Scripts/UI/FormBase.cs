using System;
using System.Collections.Generic;
using Godot;

public abstract class FormBase : IFormObject {
    private readonly Guid _id = Guid.NewGuid();
    
    protected readonly Control _menu;
    protected ControlElement _menuElement;
    protected IFormObject _topLevelLayout;
    protected FormListener _listener;

    private ICursor _cursorElement;
    private readonly SignalNode _onReadyAction;
    public bool _captureInput = true;
    private bool _registerListenerOnReady = true, _pauseGame = false;

    protected FormBase(string formName, string formPath) {
        _menu = Loader.SafeInstantiate<Control>(formPath, true);
        _menu.Name = formName;
        _onReadyAction = new SignalNode(Node.SignalName.Ready, (_, _) => {
            if (_registerListenerOnReady) _listener?.Register();
        }, this);
        GetMenu().AddChild(_onReadyAction);
    }

    public Control GetMenu() => _menu;
    public Control GetNode() => _menu;
    public Guid GetId() => _id;
    public FormListener GetListener() => _listener;

    /* --- UI CONTROL --- */
    
    public void AddToScene(CanvasLayer uiLayer) => uiLayer.AddChild(GetMenu());
    public void RemoveFromScene() => Destroy();

    /* --- FORM MANAGEMENT --- */
    
    protected abstract List<IFormObject> GetAllElements();
    protected List<T> GetElements<T>() where T : IFormObject {
        List<T> elements = new();
        foreach (IFormObject element in GetAllElements())
            if (element is T tElement) elements.Add(tElement);
        return elements;
    }
    public void Destroy() {
        if (!IsValid()) return;
        OnDestroy();

        if (this is IFocusable thisFocusable) thisFocusable.ReleaseFocus();
        
        _listener?.Unregister();
        
        foreach (IFormObject formElement in GetAllElements()) {
            formElement.Destroy();
            formElement.GetNode().QueueFree();
        }
        _onReadyAction.QueueFree();
        _menu.QueueFree();
    }
    protected T FindNode<T>(string nodePath) where T : Node => _menu.GetNode<T>(nodePath);
    public IFormObject GetTopLevelLayout() {
        if (_topLevelLayout == null) return this;
        return Equals(_topLevelLayout) ? this : _topLevelLayout;
    }
    public void SetTopLevelLayout(IFormObject layout) => _topLevelLayout = layout;
    
    /* --- BEHAVIOUR --- */

    public void SetDefaultCursor() {
        RoundCursor roundCursor = new("Cursor");
        roundCursor.SetPosition(new Vector2(0, 0));
        SetCursor(roundCursor);
    }
    public void SetCursor(ICursor cursor) {
        _cursorElement = cursor;
        _cursorElement.GetCursorElement().GetElement().SetZIndex(50);
        _menu.AddChild(cursor.GetCursorElement().GetElement());
    }
    public ICursor GetCursor() => _cursorElement;

    protected abstract void OnDestroy();
    public void SetListener(FormListener listener, bool registerNow = false) {
        _listener?.Unregister();
        _listener = listener;
        if (registerNow) _listener.Register();
    }
    public void SetRegisterListenerOnReady(bool value) => _registerListenerOnReady = value;

    public virtual void KeyboardBehaviour(Key key, bool isPressed) {}
    public virtual bool LockMovement() => true;
    public virtual bool PausesGame() => _pauseGame;
    public void SetPauseGame(bool value) => _pauseGame = value;
    public bool CaptureInput() {
        if (!IsValid()) return false;
        return Equals(GetTopLevelLayout()) ? _captureInput : GetTopLevelLayout().CaptureInput();
    }
    public void SetCaptureInput(bool value) {
        if (Equals(GetTopLevelLayout())) _captureInput = value;
        else GetTopLevelLayout().SetCaptureInput(value);
    }
    
    /* --- VALIDATION --- */
    
    protected bool IsValid() => GodotObject.IsInstanceValid(_menu) && !_menu.IsQueuedForDeletion();
    public override int GetHashCode() => GetId().GetHashCode();
    public override bool Equals(object obj) {
        if (obj is not FormBase form) return false;
        return form.GetId() == GetId();
    }
}