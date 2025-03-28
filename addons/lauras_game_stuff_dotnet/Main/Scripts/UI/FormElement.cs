using System;
using Godot;

public abstract class FormElement<T> : IFormElement where T : Control {
    private readonly Guid _guid = Guid.NewGuid();
    private T _element;
    private readonly SmartDictionary<string, SignalNode> _actions = new();
    private IFormObject _topLevelLayout;
    private bool _captureInput = true;

    protected FormElement(T element = null, Action<T> onReady = null) {
        SetElement(element);
        SetupOnReady();
        CustomOnReady(onReady);
    }

    protected FormElement(string path, Action<T> onReady = null) {
        LoadElement(path);
        SetupOnReady();
        CustomOnReady(onReady);
    }

    /* --- --- GETTERS/SETTER --- --- */
    
    public void SetElement(T element) => _element = element;
    public T GetElement() => _element;
    Control IFormElement.GetElement() => GetElement();
    public Guid GetId() => _guid;
    public IFormObject GetTopLevelLayout() => _topLevelLayout;
    public void SetTopLevelLayout(IFormObject layout) => _topLevelLayout = layout;
    public bool CaptureInput() => _captureInput;
    public void SetCaptureInput(bool value) {
        if (GetTopLevelLayout().Equals(this)) _captureInput = value;
        else GetTopLevelLayout().SetCaptureInput(value);
    }
    public bool RequiresProcess() => false;
    public Control GetNode() => GetElement();

    /* --- --- SETUP MANAGEMENT --- --- */
    
    public void Destroy() {
        if (!IsValid() || _element.IsQueuedForDeletion()) return;
        OnDestroy();
        
        foreach (SignalNode action in _actions.Values) action.QueueFree();
        _actions.Clear();
        _element.QueueFree();
    }

    protected virtual void OnDestroy() { }

    public void LoadElement(string path) {
        Node instance = Loader.SafeInstantiate<Node>(path);
        if (instance is not T element)
            throw new InvalidCastException($"ERROR: FormElement.LoadElement() : Scene at path '{path}' is not of type {typeof(T)}.");
        SetElement(element);
    }
    
    private void SetupOnReady() {
        AddAction(Node.SignalName.Ready, _ => {
            ConnectSignals();
            //EventManager.RegisterListeners(this);
        });
        ConnectSignal(Node.SignalName.Ready, true);
    }
    
    private void CustomOnReady(Action<T> onReady = null) {
        if (onReady == null) return;
        AddAction(Node.SignalName.Ready, elem => {
            Control element = elem.GetNode();
            if (element is not T control) throw new InvalidCastException($"ERROR: FormElement.OnReady() : Element is not of type {typeof(T)}. Element: {element.Name}, Type: {element.GetType()}");
            onReady?.Invoke(control);
        });
        ConnectSignal(Node.SignalName.Ready, true);
    }
    
    /* --- --- LISTENER/SIGNAL MANAGEMENT --- --- */

    public void AddAction(string signal, Action<IFormObject> action) => AddAction(signal, (formObj, _) => action(formObj));

    public void AddAction(string signal, Action<IFormObject, object[]> action) {
        if (_element == null) GD.PrintErr($"WARN: FormElement.AddAction() : No element set, cannot guarantee signal '{signal}' will be connected.");
        else if (!IsValid()) return;
        else if (!_element.HasSignal(signal)) GD.PrintErr($"WARN: FormElement.AddAction() : Signal '{signal}' not found on element '{_element.Name}'.");
        _actions.Add(signal, new SignalNode(signal, action, this));
    }

    public void RemoveAction(string signal) {
        if (!_actions.TryGetValue(signal, out SignalNode action)) return;
        action.QueueFree();
        _actions.Remove(signal);
    }
    
    private void ConnectSignal(string signal, bool clearAfterConnect = false) {
        if (!IsValid()) return;
        if (!_element.HasSignal(signal)) {
            GD.PrintErr($"ERROR: FormElement.ConnectSignal() : Signal '{signal}' not found on element '{_element.Name}'.");
            return;
        }
        if (!_actions.TryGetValue(signal, out SignalNode action)) {
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
    
    /* --- --- VALIDATION --- --- */

    protected bool IsValid() => GodotObject.IsInstanceValid(_element) && !_element.IsQueuedForDeletion();
    
    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return GetId() == ((FormElement<T>) obj).GetId();
    }
    public override int GetHashCode() => GetId().GetHashCode();
}