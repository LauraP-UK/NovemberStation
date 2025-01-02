using Godot;

public class MouseClickEvent : EventBase<Vector2> {

    private readonly MouseButton _mouseButton;
    private readonly bool _pressed;
    private readonly Vector2 _position;

    public MouseClickEvent(MouseButton mouseButton, bool pressed, Vector2 position) {
        _mouseButton = mouseButton;
        _pressed = pressed;
        _position = position;
    }

    public MouseButton GetMouseButton() => _mouseButton;
    public bool IsPressed() => _pressed;
    public Vector2 GetPosition() => _position;
    public override Vector2 GetAdditionalContext() => GetPosition();
}