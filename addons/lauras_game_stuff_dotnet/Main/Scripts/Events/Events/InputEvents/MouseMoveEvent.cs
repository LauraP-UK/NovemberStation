using Godot;

public class MouseMoveEvent : EventBase<Vector2> {
    private readonly Vector2 _position;
    private readonly Vector2 _delta;

    public MouseMoveEvent(Vector2 position, Vector2 delta) {
        _position = position;
        _delta = delta;
    }
    
    public Vector2 GetPosition() => _position;
    public Vector2 GetDelta() => _delta;

    public override Vector2 GetAdditionalContext() {
        return GetDelta();
    }
}