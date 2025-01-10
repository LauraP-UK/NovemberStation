
using System.Collections.Generic;
using Godot;

public interface ILayoutElement : IFormObject {
    public Container GetContainer();
    public Container Build(Container parent, HashSet<ILayoutElement> processedLayouts, bool warnOnCircularReference = true);
}