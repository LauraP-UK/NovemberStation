
using System;
using Godot;

public class ScrollContainerLayout : LayoutElement<ScrollContainer> {
    
    public ScrollContainerLayout(ScrollContainer container = null, Action<ScrollContainer> onReady = null) : base(container, onReady) { }
    public ScrollContainerLayout(string path, Action<ScrollContainer> onReady = null) : base(path, onReady) { }
    
    public void FollowFocus(bool follow) => GetContainer().SetFollowFocus(follow);
    public void SetHScrollMode(ScrollContainer.ScrollMode value) => GetContainer().SetHorizontalScrollMode(value);
    public void SetVScrollMode(ScrollContainer.ScrollMode value) => GetContainer().SetVerticalScrollMode(value);
    public void SetScrollDeadZone(int value) => GetContainer().SetDeadzone(value);
    
    public void SetHScroll(int value) => GetContainer().SetHScroll(value);
    public void SetVScroll(int value) => GetContainer().SetVScroll(value);
    public void SetHCustomStep(float value) => GetContainer().SetHorizontalCustomStep(value);
    public void SetVCustomStep(float value) => GetContainer().SetVerticalCustomStep(value);
}