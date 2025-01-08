
using System;
using Godot;

public abstract class FormElement<T> : Listener where T : Control {
    
    protected T _element;
    protected readonly SmartDictionary<string, ActionNode> _actions = new();
    
    public void SetElement(T element) => _element = element;
    public T GetElement() => _element;

    public void AddAction(string signal, Action action) => AddAction(signal, _ => action());
    public void AddAction(string signal, Action<object[]> action) {
        if (!_element.HasSignal(signal))
            throw new ArgumentException($"ERROR: FormElement.AddAction() : Signal '{signal}' not found on element '{_element.Name}'.");
        _actions.Add(signal, new ActionNode(signal, action));
    }

    public void RemoveAction(string signal) {
        if (!_actions.TryGetValue(signal, out ActionNode action)) return;
        action.QueueFree();
        _actions.Remove(signal);
    }

    protected void ConnectSignals() {
        foreach (string signal in _actions.Keys) {
            ActionNode action = _actions[signal];
            _element.Connect(signal, new Callable(action, nameof(action.RunAction)));
        }
    }
}