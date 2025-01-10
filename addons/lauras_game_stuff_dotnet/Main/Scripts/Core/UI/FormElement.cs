
using System;
using System.Linq;
using Godot;
using NovemberStation.Main;

public abstract class FormElement<T> : Listener, IFormElement where T : Control {
    private T _element;
    private readonly SmartDictionary<string, ActionNode> _actions = new();

    protected FormElement(T element = null) => SetElement(element);
    protected FormElement(string path, Action<T> initialiser = null) => LoadElement(path, initialiser);
    
    public void SetElement(T element) => _element = element;
    public T GetElement() => _element;
    Control IFormElement.GetElement() => GetElement();

    protected void AddAction(string signal, Action action) => AddAction(signal, _ => action());
    protected void AddAction(string signal, Action<object[]> action) {
        if (_element == null) GD.PrintErr($"WARN: FormElement.AddAction() : No element set, cannot guarantee signal '{signal}' will be connected.");
        else if (!_element.HasSignal(signal)) GD.PrintErr($"WARN: FormElement.AddAction() : Signal '{signal}' not found on element '{_element.Name}'.");
        _actions.Add(signal, new ActionNode(signal, action, _element));
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
    
    public void LoadElement(string path, Action<T> initialiser = null) {
        if (!ResourceLoader.Exists(path)) {
            GD.PrintErr($"ERROR: FormElement.LoadElement() : No scene found at path '{path}', cannot load {typeof(T)}.");
            return;
        }
        
        PackedScene packedScene = GD.Load<PackedScene>(path);
        if (packedScene == null)
            throw new NullReferenceException($"ERROR: FormElement.LoadElement() : No scene found at path '{path}', cannot load {typeof(T)}.");
        
        Node instance = packedScene.Instantiate();
        if (instance is not T element)
            throw new InvalidCastException($"ERROR: FormElement.LoadElement() : Scene at path '{path}' is not of type {typeof(T)}.");
        
        initialiser?.Invoke(element);
        SetElement(element);
        OnElementReady(element);
    }
    
    protected virtual void OnElementReady(T element) { }
}