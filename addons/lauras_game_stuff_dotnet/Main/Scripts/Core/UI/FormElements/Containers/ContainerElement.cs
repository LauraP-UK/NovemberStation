using System;
using Godot;

namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class ContainerElement : FormElement<Container> {
    public ContainerElement(Container container = null) : base(container) { }
    public ContainerElement(string path, Action<Container> initialiser = null) : base(path, initialiser) { }
}