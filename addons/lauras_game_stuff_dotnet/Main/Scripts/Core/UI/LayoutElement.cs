
using System;
using System.Collections.Generic;
using Godot;

public abstract class LayoutElement<T> : Listener, ILayoutElement where T : Container {
    private Guid _uuid;
    private SmartSet<IFormObject> _elements = new();
    private T _container;
    
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

    public Container Build(Container parent = null, HashSet<ILayoutElement> processedLayouts = null, bool warnOnCircularReference = true) {
        if (processedLayouts != null && processedLayouts.Contains(this)) {
            if (warnOnCircularReference) {
                GD.PrintErr($"WARNING: LayoutElement.Build() : Circular reference detected, skipping... Type is: {GetType()} - ({typeof(T)}).");
                return parent;
            }
            throw new InvalidOperationException($"ERROR: LayoutElement.Build() : Circular reference detected, cannot build layout. Type is: {GetType()} - ({typeof(T)}).");
        }

        T thisContainer = GetContainer();

        parent ??= thisContainer;
        if (parent == null)
            throw new NullReferenceException($"ERROR: LayoutElement.Build() : Parent is null, cannot build layout. Type is: {GetType()} - ({typeof(T)}).");
        GD.Print($"{GetUuid()} : Building layout '{thisContainer.GetInstanceId()}' with type '{GetType()}' - ({typeof(T)}).");
        GD.Print($"{GetUuid()} : I have {GetElements().Count} elements.");
        
        processedLayouts ??= new HashSet<ILayoutElement>();
        processedLayouts.Add(this);
        
        PreBuild(thisContainer);
        
        int debugCounter = 0;
        
        foreach (IFormObject formElementBase in GetElements()) {
            GD.Print($"{GetUuid()} : Building element {debugCounter} : '{formElementBase.GetType()}'...");
            switch (formElementBase) {
                case IFormElement formElement: {
                    GD.Print($"{GetUuid()} : - Building form element '{formElement.GetType()}'...");
                    Control element = formElement.GetElement();
                    if (element == null) {
                        GD.PrintErr($"ERROR: LayoutElement.Build() : Element is null, cannot add to parent '{parent.GetInstanceId()}'.");
                        continue;
                    }
                    GD.Print($"{GetUuid()} : - Connecting signals for form element '{formElement.GetType()}'...");
                    formElement.ConnectSignals();
                    GD.Print($"{GetUuid()} : - Adding form element '{formElement.GetType()}' to parent '{parent.GetInstanceId()}'...");
                    thisContainer.AddChild(element);
                    GD.Print($"{GetUuid()} : - Form element '{formElement.GetType()}' added to parent '{parent.GetInstanceId()}'.");
                    break;
                }
                case ILayoutElement layoutElement:
                    GD.Print($"{GetUuid()} : - Found a layout element '{layoutElement.GetType()}'... Building this now...");
                    Container built = layoutElement.Build(thisContainer, processedLayouts);
                    if (built == null) {
                        GD.PrintErr($"ERROR: LayoutElement.Build() : Layout is null, cannot add to parent '{parent.GetInstanceId()}'.");
                        continue;
                    }
                    GD.Print($"{GetUuid()} : - Adding layout element '{layoutElement.GetType()}' to parent '{parent.GetInstanceId()}'...");
                    thisContainer.AddChild(built); // Add child layout to parent
                    GD.Print($"{GetUuid()} : - Layout element '{layoutElement.GetType()}' added to parent '{parent.GetInstanceId()}'.");
                    break;
            }
            debugCounter++;
        }
        
        PostBuild(thisContainer);
        GD.Print($"{GetUuid()} : I ({GetType()}) built successfully.");
        GD.Print($"{GetUuid()} : Returning parent '{parent.GetInstanceId()}'.");
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