
using System;
using Godot;

public class CenterContainerLayout : LayoutElement<CenterContainer> {
    
    public CenterContainerLayout(CenterContainer container = null, Action<CenterContainer> onReady = null) : base(container, onReady) { }
    public CenterContainerLayout(string path, Action<CenterContainer> onReady = null) : base(path, onReady) { }
    
}