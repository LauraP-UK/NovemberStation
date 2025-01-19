using System;
using Godot;

namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class ControlElement : FormElement<Control> {
    public ControlElement(Control container = null, Action<Control> onReady = null) : base(container, onReady) { }
    public ControlElement(string path, Action<Control> onReady = null) : base(path, onReady) { }
}