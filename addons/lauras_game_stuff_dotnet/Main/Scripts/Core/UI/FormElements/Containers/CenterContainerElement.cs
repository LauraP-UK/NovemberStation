using System;
using Godot;

namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class CenterContainerElement : FormElement<CenterContainer> {
    public CenterContainerElement(CenterContainer container = null, Action<CenterContainer> onReady = null) : base(container, onReady) { }
    public CenterContainerElement(string path, Action<CenterContainer> onReady = null) : base(path, onReady) { }
}