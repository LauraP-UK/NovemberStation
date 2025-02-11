using Godot;

public class InputController {
    public static void ProcessInput(InputEvent @event) {
        switch (@event) {
            case InputEventKey { Pressed: true } eventKey:
                new KeyPressEvent(eventKey.Keycode).Fire();
                break;
            case InputEventKey eventKey:
                new KeyReleaseEvent(eventKey.Keycode).Fire();
                break;
            case InputEventMouseButton eventMouseButton: {
                MouseButton button = eventMouseButton.ButtonIndex;
                bool pressed = eventMouseButton.Pressed;
                new MouseInputEvent(button, pressed, eventMouseButton.Position).Fire();
                break;
            }
            case InputEventMouseMotion eventMouseMotion:
                new MouseMoveEvent(eventMouseMotion.Position, eventMouseMotion.Relative).Fire();
                break;
        }
    }
}