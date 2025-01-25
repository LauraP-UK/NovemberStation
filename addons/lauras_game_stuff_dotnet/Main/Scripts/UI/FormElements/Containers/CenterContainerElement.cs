using System;
using Godot;

public class CenterContainerElement : FormElement<CenterContainer> {
    public CenterContainerElement(CenterContainer container = null, Action<CenterContainer> onReady = null) : base(container, onReady) { }
    public CenterContainerElement(string path, Action<CenterContainer> onReady = null) : base(path, onReady) { }
}