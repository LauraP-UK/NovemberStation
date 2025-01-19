using System;
using Godot;

namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class ScrollContainerElement : FormElement<ScrollContainer> {
    public ScrollContainerElement(ScrollContainer container = null, Action<ScrollContainer> onReady = null) : base(container, onReady) { }
    public ScrollContainerElement(string path, Action<ScrollContainer> onReady = null) : base(path, onReady) { }
    
    public void FollowFocus(bool follow) => GetElement().SetFollowFocus(follow);
    public void SetHScrollMode(ScrollContainer.ScrollMode value) => GetElement().SetHorizontalScrollMode(value);
    public void SetVScrollMode(ScrollContainer.ScrollMode value) => GetElement().SetVerticalScrollMode(value);
    public void SetScrollDeadZone(int value) => GetElement().SetDeadzone(value);
    
    public void SetHScroll(int value) => GetElement().SetHScroll(value);
    public void SetVScroll(int value) => GetElement().SetVScroll(value);
    public void SetHCustomStep(float value) => GetElement().SetHorizontalCustomStep(value);
    public void SetVCustomStep(float value) => GetElement().SetVerticalCustomStep(value);
}