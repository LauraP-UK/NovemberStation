
using System;
using Godot;

public class ContainerLayout : LayoutElement<Container> {
    public ContainerLayout(Container container = null) : base(container) { }
    public ContainerLayout(string path, Action<Container> initialiser = null) : base(path, initialiser) { }
}