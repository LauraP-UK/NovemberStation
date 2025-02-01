
using static BaseActionDefault.MouseType;

public abstract class BaseActionDefault {
    public enum MouseType {
        DOWN,
        UP,
        BOTH,
        NONE
    }
    
    protected static bool CanRun(IEventBase ev, MouseType validMouseType) {
        if (ev is not MouseInputEvent mouseEvent) return true;
        bool isPressed = mouseEvent.IsPressed();
        switch (validMouseType) {
            case NONE:
                return false;
            case BOTH:
                return true;
            case DOWN when !isPressed:
            case UP when isPressed:
                return false;
        }
        return true;
    }
}