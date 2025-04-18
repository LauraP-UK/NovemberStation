
using Godot;

public class PlayerUseClickEvent : ActorEventBase {

    private readonly MouseInputEvent _eventInfo;
    
    public PlayerUseClickEvent(MouseButton mouseButton, bool pressed, Vector2 position) {
        SetActor(GameManager.GetPlayer());
        _eventInfo = new MouseInputEvent(mouseButton, pressed, position);
    }
    
    public MouseButton GetMouseButton() => _eventInfo.GetMouseButton();
    public bool IsPressed() => _eventInfo.IsPressed();
    public Vector2 GetPosition() => _eventInfo.GetAdditionalContext();
    
}