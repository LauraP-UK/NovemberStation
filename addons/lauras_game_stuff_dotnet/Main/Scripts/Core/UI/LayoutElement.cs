
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public abstract class LayoutElement<T> : Listener, ILayoutElement where T : Control {
    private Guid _uuid;
    private SmartSet<IFormObject> _elements = new();
    private readonly SmartDictionary<string, ActionNode> _actions = new();
    private T _container;
    private ILayoutElement _topLevelLayout;
    
    public LayoutElement(T container = null, Action<T> onReady = null) {
        SetContainer(container);
        _uuid = Guid.NewGuid();
        OnReady(onReady);
    }
    
    public LayoutElement(string path, Action<T> onReady = null) {
        LoadContainer(path);
        _uuid = Guid.NewGuid();
        OnReady(onReady);
    }

    public void SetContainer(T container) => _container = container;
    public T GetContainer() => _container;
    Control ILayoutElement.GetContainer() => GetContainer();
    public Control GetNode() => GetContainer();

    public void AddElement(IFormObject element) => _elements.Add(element);
    public void RemoveElement(IFormObject element) => _elements.Remove(element);
    public SmartSet<IFormObject> GetElements() => _elements;
    public string GetUuid() => _uuid.ToString();
    public ILayoutElement GetTopLevelLayout() => _topLevelLayout;
    public void SetTopLevelLayout(ILayoutElement layout) => _topLevelLayout = layout;

    private void OnReady(Action<T> onReady) {
        if (onReady == null) return;
        AddAction(Node.SignalName.Ready, elem => {
            Control container = ((ILayoutElement)elem).GetContainer();
            onReady?.Invoke(container as T);
        });
        ConnectSignal(Node.SignalName.Ready, true);
    }
    
    public void OnResize(Action<IFormObject> onResize) {
        if (onResize == null) return;
        AddAction(Control.SignalName.Resized, _ => onResize(this));
    }
    
    private void ConnectSignal(string signal, bool clearAfterConnect = false) {
        if (!IsValid()) return;
        if (!_container.HasSignal(signal)) {
            GD.PrintErr($"ERROR: LayoutElement.ConnectSignal() : Signal '{signal}' not found on element '{_container.Name}'.");
            return;
        }
        if (!_actions.TryGetValue(signal, out ActionNode action)) {
            GD.PrintErr($"ERROR: LayoutElement.ConnectSignal() : Action for signal '{signal}' not found.");
            return;
        }

        if (_container.IsConnected(signal, action.GetCallable())) {
            GD.Print($"WARN: LayoutElement.ConnectSignal() : Signal '{signal}' is already connected.");
            return;
        }
        
        _container.AddChild(action);
        if (clearAfterConnect) _actions.Remove(signal);
    }

    public void ConnectSignals() {
        if (!IsValid()) return;
        foreach (string signal in _actions.Keys) ConnectSignal(signal);
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
        if (_container == null) GD.PrintErr($"WARN: LayoutElement.AddAction() : No container set, cannot guarantee signal '{signal}' will be connected.");
        else if (!IsValid()) return;
        else if (!_container.HasSignal(signal)) GD.PrintErr($"WARN: LayoutElement.AddAction() : Signal '{signal}' not found on container '{_container.Name}'.");
        _actions.Add(signal, new ActionNode(signal, action, this));
    }

    public TType Clone<TType>() where TType : LayoutElement<T> {
        LayoutElement<T> clone = (LayoutElement<T>) MemberwiseClone();
        clone._uuid = Guid.NewGuid();
        clone._elements = _elements.Clone();
        clone._container = (T) _container.Duplicate();
        return (TType) clone;
    }
    
    public void LoadContainer(string path) {
        Node instance = Loader.SafeInstantiate<Node>(path);
        if (instance is not T container)
            throw new InvalidCastException($"ERROR: LayoutElement.LoadContainer() : Scene at path '{path}' is not of type {typeof(T)}.");
        
        SetContainer(container);
    }
    
    /* --- Build --- */

    public Control Build(ILayoutElement parent = null, HashSet<ILayoutElement> processedLayouts = null, bool warnOnCircularReference = true) {
        if (processedLayouts != null && processedLayouts.Contains(this)) {
            if (warnOnCircularReference) {
                GD.PrintErr($"WARNING: LayoutElement.Build() : Circular reference detected, skipping... Type is: {GetType()} - ({typeof(T)}).");
                return parent?.GetContainer();
            }
            throw new InvalidOperationException($"ERROR: LayoutElement.Build() : Circular reference detected, cannot build layout. Type is: {GetType()} - ({typeof(T)}).");
        }

        T thisContainer = GetContainer();
        if (!IsValid()) {
            GD.PrintErr($"ERROR: LayoutElement.Build() : Container is no longer valid, cannot build layout. Type is: {GetType()} - ({typeof(T)}).");
            return null;
        }
        
        SetTopLevelLayout(parent == null ? this : parent.GetTopLevelLayout());

        parent ??= this;
        if (parent.GetContainer() == null)
            throw new NullReferenceException($"ERROR: LayoutElement.Build() : Parent container is null, cannot build layout. Type is: {GetType()} - ({typeof(T)}).");
        
        processedLayouts ??= new HashSet<ILayoutElement>();
        processedLayouts.Add(this);
        
        ConnectSignals();
        
        PreBuild(thisContainer);
        
        foreach (IFormObject formElementBase in GetElements()) {
            switch (formElementBase) {
                case IFormElement formElement: {
                    Control element = formElementBase.GetNode();
                    if (element == null) {
                        GD.PrintErr($"ERROR: LayoutElement.Build() : Element is null, cannot add to parent '{parent.GetUuid()}'.");
                        continue;
                    }
                    formElement.SetTopLevelLayout(GetTopLevelLayout());
                    thisContainer.AddChild(element);
                    GD.Print($"Top level layout is: {GetTopLevelLayout().GetUuid()}");
                    break;
                }
                case ILayoutElement layoutElement:
                    Control built = layoutElement.Build(this, processedLayouts);
                    if (built == null) {
                        GD.PrintErr($"ERROR: LayoutElement.Build() : Layout is null, cannot add to parent '{parent.GetUuid()}'.");
                        continue;
                    }
                    thisContainer.AddChild(built);
                    break;
            }
        }
        
        PostBuild(thisContainer);
        return thisContainer;
    }

    public virtual void PreBuild(Control container) { }
    public virtual void PostBuild(Control container) { }

    protected bool IsValid() => GodotObject.IsInstanceValid(_container) && !_container.IsQueuedForDeletion();
    
    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return _uuid == ((LayoutElement<T>) obj)._uuid;
    }
    public override int GetHashCode() => _uuid.GetHashCode();
}