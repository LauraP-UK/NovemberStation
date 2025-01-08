using Godot;

public class InputController {
    public static void ProcessInput(InputEvent @event) {
        if (@event is InputEventKey eventKey) {
            if (eventKey.Pressed)
                new KeyPressEvent(eventKey.Keycode).Fire();
            else
                new KeyReleaseEvent(eventKey.Keycode).Fire();
        }
        else if (@event is InputEventMouseButton eventMouseButton) {
            MouseButton button = eventMouseButton.ButtonIndex;
            bool pressed = eventMouseButton.Pressed;
            new MouseClickEvent(button, pressed, eventMouseButton.Position).Fire();
        }
        else if (@event is InputEventMouseMotion eventMouseMotion) {
            new MouseMoveEvent(eventMouseMotion.Position, eventMouseMotion.Relative).Fire();
        }
    }
}