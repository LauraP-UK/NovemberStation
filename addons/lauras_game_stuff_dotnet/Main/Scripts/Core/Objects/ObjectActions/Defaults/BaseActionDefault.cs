
public abstract class BaseActionDefault : Listener {
    public enum MouseType {
        DOWN,
        UP,
        BOTH,
        NONE
    }
    
    public enum KeyType {
        DOWN,
        UP,
        BOTH,
        NONE
    }

    protected static bool CanRun(IEventBase ev, KeyType validKeyType) {
        if (ev is not KeyPressEvent && ev is not KeyReleaseEvent) return false;
        switch (validKeyType) {
            case KeyType.NONE:
                return false;
            case KeyType.BOTH:
                return true;
            default:
                switch (ev) {
                    case KeyPressEvent when validKeyType == KeyType.DOWN:
                    case KeyReleaseEvent when validKeyType == KeyType.UP:
                        return true;
                    default:
                        return false;
                }
        }
    }
    
    protected static bool CanRun(IEventBase ev, MouseType validMouseType) {
        if (ev is not MouseInputEvent mouseEvent) return false;
        bool isPressed = mouseEvent.IsPressed();
        switch (validMouseType) {
            case MouseType.NONE:
                return false;
            case MouseType.BOTH:
                return true;
            case MouseType.DOWN when !isPressed:
            case MouseType.UP when isPressed:
                return false;
        }
        return true;
    }
}