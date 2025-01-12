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
    Control IFormElement.GetElement() => GetElement();
    public ILayoutElement GetTopLevelLayout() => _topLevelLayout;
    public void SetTopLevelLayout(ILayoutElement layout) => _topLevelLayout = layout;

    private void OnReady(Action<T> onReady = null) {
        if (onReady != null) GD.Print($"Adding onReady action for {typeof(T)}");
        AddAction(Node.SignalName.Ready, elem => {
            Control element = ((IFormElement)elem).GetElement();
            onReady?.Invoke(element as T);
        });
    }

    public void OnResize(Action<IFormObject> onResize) {
        if (onResize == null) return;
        AddAction(Control.SignalName.Resized, _ => onResize(this));
    }

    [EventListener]
    protected void OnWindowResize(WindowResizeEvent ev, Vector2 v) {
        foreach (string signal in _actions.Keys.Where(signal => signal == Control.SignalName.Resized)) {
            _actions[signal].RunActionNoArgs();
            return;
        }
    }

    protected void AddAction(string signal, Action<IFormObject> action) {
        AddAction(signal, (formObj, _) => action(formObj));
    }

    protected void AddAction(string signal, Action<IFormObject, object[]> action) {
        if (_element == null) GD.PrintErr($"WARN: FormElement.AddAction() : No element set, cannot guarantee signal '{signal}' will be connected.");
        else if (!_element.HasSignal(signal)) GD.PrintErr($"WARN: FormElement.AddAction() : Signal '{signal}' not found on element '{_element.Name}'.");
        _actions.Add(signal, new ActionNode(signal, action, this));
    }

    public void RemoveAction(string signal) {
        if (!_actions.TryGetValue(signal, out ActionNode action)) return;
        action.QueueFree();
        _actions.Remove(signal);
    }

    public void ConnectSignals() {
        foreach (string signal in _actions.Keys) {
            ActionNode action = _actions[signal];
            if (!_element.HasSignal(signal)) {
                GD.PrintErr($"ERROR: FormElement.ConnectSignals() : Signal '{signal}' not found on element '{_element.Name}'.");
                continue;
            }
            _element.AddChild(action);
        }
    }

    public void LoadElement(string path) {
        Node instance = Loader.SafeInstantiate<Node>(path);
        if (instance is not T element)
            throw new InvalidCastException($"ERROR: FormElement.LoadElement() : Scene at path '{path}' is not of type {typeof(T)}.");

        SetElement(element);
    }
}