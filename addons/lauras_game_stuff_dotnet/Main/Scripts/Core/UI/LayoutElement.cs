
using System;
using System.Collections.Generic;
using Godot;

public abstract class LayoutElement<T> : Listener, ILayoutElement where T : Container {
    private Guid _uuid;
    private SmartSet<IFormObject> _elements = new();
    private T _container;
    private ILayoutElement _topLevelLayout;
    
    public LayoutElement(T container = null) {
        SetContainer(container);
        _uuid = Guid.NewGuid();
    }
    
    public LayoutElement(string path, Action<T> initialiser = null) {
        LoadContainer(path, initialiser);
        _uuid = Guid.NewGuid();
    }

    public void SetContainer(T container) => _container = container;
    public T GetContainer() => _container;
    Container ILayoutElement.GetContainer() => GetContainer();
    
    public void AddElement(IFormObject element) => _elements.Add(element);
    public void RemoveElement(IFormObject element) => _elements.Remove(element);
    public SmartSet<IFormObject> GetElements() => _elements;
    public string GetUuid() => _uuid.ToString();
    public ILayoutElement GetTopLevelLayout() => _topLevelLayout;
    public void SetTopLevelLayout(ILayoutElement layout) => _topLevelLayout = layout;

    public TType Clone<TType>() where TType : LayoutElement<T> {
        LayoutElement<T> clone = (LayoutElement<T>) MemberwiseClone();
        clone._uuid = Guid.NewGuid();
        clone._elements = _elements.Clone();
        clone._container = (T) _container.Duplicate();
        return (TType) clone;
    }
    
    public void LoadContainer(string path, Action<T> initialiser = null) {
        if (!ResourceLoader.Exists(path)) {
            GD.PrintErr($"ERROR: LayoutElement.LoadContainer() : No scene found at path '{path}', cannot load {typeof(T)}.");
            return;
        }
        
        PackedScene packedScene = GD.Load<PackedScene>(path);
        if (packedScene == null)
            throw new NullReferenceException($"ERROR: LayoutElement.LoadContainer() : No scene found at path '{path}', cannot load {typeof(T)}.");
        
        Node instance = packedScene.Instantiate();
        if (instance is not T container)
            throw new InvalidCastException($"ERROR: LayoutElement.LoadContainer() : Scene at path '{path}' is not of type {typeof(T)}.");
        
        initialiser?.Invoke(container);
        SetContainer(container);
    }
    
    /* --- Build --- */

    public Container Build(ILayoutElement parent = null, HashSet<ILayoutElement> processedLayouts = null, bool warnOnCircularReference = true) {
        if (processedLayouts != null && processedLayouts.Contains(this)) {
            if (warnOnCircularReference) {
                GD.PrintErr($"WARNING: LayoutElement.Build() : Circular reference detected, skipping... Type is: {GetType()} - ({typeof(T)}).");
                return parent?.GetContainer();
            }
            throw new InvalidOperationException($"ERROR: LayoutElement.Build() : Circular reference detected, cannot build layout. Type is: {GetType()} - ({typeof(T)}).");
        }

        T thisContainer = GetContainer();
        
        SetTopLevelLayout(parent == null ? this : parent.GetTopLevelLayout());

        parent ??= this;
        if (parent.GetContainer() == null)
            throw new NullReferenceException($"ERROR: LayoutElement.Build() : Parent container is null, cannot build layout. Type is: {GetType()} - ({typeof(T)}).");
        
        processedLayouts ??= new HashSet<ILayoutElement>();
        processedLayouts.Add(this);
        
        GD.Print($"Building layout: {GetUuid()} - {GetType()} - {typeof(T)}");
        
        PreBuild(thisContainer);
        
        foreach (IFormObject formElementBase in GetElements()) {
            switch (formElementBase) {
                case IFormElement formElement: {
                    Control element = formElement.GetElement();
                    if (element == null) {
                        GD.PrintErr($"ERROR: LayoutElement.Build() : Element is null, cannot add to parent '{parent.GetUuid()}'.");
                        continue;
                    }
                    formElement.ConnectSignals();
                    formElement.SetTopLevelLayout(GetTopLevelLayout());
                    thisContainer.AddChild(element);
                    GD.Print($"Top level layout is: {GetTopLevelLayout().GetUuid()}");
                    break;
                }
                case ILayoutElement layoutElement:
                    Container built = layoutElement.Build(this, processedLayouts);
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

    public virtual void PreBuild(Container container) { }
    public virtual void PostBuild(Container container) { }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        return _uuid == ((LayoutElement<T>) obj)._uuid;
    }
    
    public override int GetHashCode() => _uuid.GetHashCode();
}