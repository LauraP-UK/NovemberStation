
using System.Collections.Generic;
using Godot;

public interface ILayoutElement : IFormObject {
    public Container GetContainer();
    public string GetUuid();
    public Container Build(ILayoutElement parent, HashSet<ILayoutElement> processedLayouts, bool warnOnCircularReference = true);
}