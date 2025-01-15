
using System.Collections.Generic;
using Godot;

public interface ILayoutElement : IFormObject {
    public Control GetContainer();
    public string GetUuid();
    public Control Build(ILayoutElement parent = null, HashSet<ILayoutElement> processedLayouts = null, bool warnOnCircularReference = true);
}