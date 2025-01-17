using System;
using System.Linq;
using Godot;

public abstract class FormElement<T> : Listener, IFormElement where T : Control {
    private T _element;
    private readonly SmartDictionary<string, ActionNode> _actions = new();
    private ILayoutElement _topLevelLayout;

    protected FormElement(T element = null, Action<T> onReady = null) {
        SetElement(element);
        OnReady(onReady);
    }

    protected FormElement(string path, Action<T> onReady = null) {
        LoadElement(path);
        OnReady(onReady);
    }

    public void SetElement(T element) => _element = element;

    public T GetElement() => _element;
    Control IFormElement.GetElement() => GetNode();
    public ILayoutElement GetTopLevelLayout() => _topLevelLayout;
    public void SetTopLevelLayout(ILayoutElement layout) => _topLevelLayout = layout;
    public Control GetNode() => GetElement();

    private void OnReady(Action<T> onReady = null) {
        AddAction(Node.SignalName.Ready, elem => {
            Control element = elem.GetNode();
            
            if (element is not T control) throw new InvalidCastException($"ERROR: FormElement.OnReady() : Element is not of type {typeof(T)}. Element: {element.Name}, Type: {element.GetType()}");
            onReady?.Invoke(control);
            ConnectSignals();
        });
        ConnectSignal(Node.SignalName.Ready, true);
    }

    public void OnResize(Action<IFormObject> onResize) {
        if (onResize == null) return;
        AddAction(Control.SignalName.Resized, _ => onResize(this));
    }

    public void Destroy() {
        if (!IsValid() || _element.IsQueuedForDeletion()) return;
        EventManager.I().UnregisterByOwner(this);
        foreach (ActionNode action in _actions.Values) action.QueueFree();
        _actions.Clear();
        _element.QueueFree();
    }

    [EventListener]
    protected void OnWindowResize(WindowResizeEvent ev, Vector2 v) {
        foreach (string signal in _actions.Keys.Where(signal => signal == Control.SignalName.Resized)) {
            _actions[signal].RunActionNoArgs();
            return;
        }
    }

    public void AddAction(string signal, Action<IFormObject> action) {
        AddAction(signal, (formObj, _) => action(formObj));
    }

    public void AddAction(string signal, Action<IFormObject, object[]> action) {
        if (_element == null) GD.PrintErr($"WARN: FormElement.AddAction() : No element set, cannot guarantee signal '{signal}' will be connected.");
        else if (!IsValid()) return;
        else if (!_element.HasSignal(signal)) GD.PrintErr($"WARN: FormElement.AddAction() : Signal '{signal}' not found on element '{_element.Name}'.");
        _actions.Add(signal, new ActionNode(signal, action, this));
    }

    public void RemoveAction(string signal) {
        if (!_actions.TryGetValue(signal, out ActionNode action)) return;
        action.QueueFree();
        _actions.Remove(signal);
    }
    
    private void ConnectSignal(string signal, bool clearAfterConnect = false) {
        if (!IsValid()) return;
        if (!_element.HasSignal(signal)) {
            GD.PrintErr($"ERROR: FormElement.ConnectSignal() : Signal '{signal}' not found on element '{_element.Name}'.");
            return;
        }
        if (!_actions.TryGetValue(signal, out ActionNode action)) {
            GD.PrintErr($"ERROR: FormElement.ConnectSignal() : Action for signal '{signal}' not found.");
            return;
        }

        if (_element.IsConnected(signal, action.GetCallable())) {
            GD.Print($"WARN: FormElement.ConnectSignal() : Signal '{signal}' is already connected.");
            return;
        }
        
        _element.AddChild(action);
        if (clearAfterConnect) _actions.Remove(signal);
    }

    public void ConnectSignals() {
        if (!IsValid()) return;
        foreach (string signal in _actions.Keys) ConnectSignal(signal);
    }

    public void LoadElement(string path) {
        Node instance = Loader.SafeInstantiate<Node>(path);
        if (instance is not T element)
            throw new InvalidCastException($"ERROR: FormElement.LoadElement() : Scene at path '{path}' is not of type {typeof(T)}.");
        SetElement(element);
    }

    protected bool IsValid() => GodotObject.IsInstanceValid(_element) && !_element.IsQueuedForDeletion();
}