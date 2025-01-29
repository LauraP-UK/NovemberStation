
using Godot;
using static ActionBase.MouseType;

public abstract class ActionBase {
    public enum MouseType {
        DOWN,
        UP,
        BOTH,
        NONE
    }
    
    private ObjectActions.ActionType _actionType;
    
    private readonly string _actionName;
    private readonly int _index;

    protected ActionBase(ObjectActions.ActionType actionType, string actionName, int index) {
        _actionType = actionType;
        _actionName = actionName;
        _index = index;
    }

    public abstract void Invoke<T>(ActorBase actorBase, T node, IEventBase ev) where T : Node3D;
    public int GetIndex() => _index;
    public string GetActionName() => _actionName;

    protected bool CanRun(IEventBase ev) {
        if (ev is MouseInputEvent mouseEvent) {
            MouseType mouseType = GetMouseType();
            bool isPressed = mouseEvent.IsPressed();
            switch (mouseType) {
                case NONE:
                    return false;
                case BOTH:
                    return true;
                case DOWN when !isPressed:
                case UP when isPressed:
                    return false;
            }
        }
        return true;
    }
    protected abstract MouseType GetMouseType();
}