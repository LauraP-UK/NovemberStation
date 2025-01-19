using System;
using Godot;

namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class HBoxContainerElement : FormElement<HBoxContainer> {
    public HBoxContainerElement(HBoxContainer container = null, Action<HBoxContainer> onReady = null) : base(container, onReady) { }
    public HBoxContainerElement(string path, Action<HBoxContainer> onReady = null) : base(path, onReady) { }
}