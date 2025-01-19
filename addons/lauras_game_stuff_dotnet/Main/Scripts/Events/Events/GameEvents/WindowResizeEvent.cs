
using Godot;

public class WindowResizeEvent : EventBase<Vector2> {
    private Vector2 _size;
    public void SetSize(Vector2 size) => _size = size;
    public override Vector2 GetAdditionalContext() => _size;
}