
using Godot;

public interface ICursor {
    public ControlElement GetCursorElement();
    public void SetPosition(Vector2 position);
}