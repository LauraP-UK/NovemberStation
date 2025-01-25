using System;
using Godot;

public class ContainerElement : FormElement<Container> {
    public ContainerElement(Container container = null) : base(container) { }
    public ContainerElement(string path, Action<Container> initialiser = null) : base(path, initialiser) { }
}