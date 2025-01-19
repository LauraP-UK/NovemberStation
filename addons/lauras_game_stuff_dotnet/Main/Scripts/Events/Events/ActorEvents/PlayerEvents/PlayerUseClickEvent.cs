
using Godot;
using NovemberStation.Main;

public class PlayerUseClickEvent : ActorEventBase {

    private readonly MouseClickEvent _eventInfo;
    
    public PlayerUseClickEvent(MouseButton mouseButton, bool pressed, Vector2 position) {
        SetActor(GameManager.I().GetPlayer());
        _eventInfo = new MouseClickEvent(mouseButton, pressed, position);
    }
    
    public MouseButton GetMouseButton() => _eventInfo.GetMouseButton();
    public bool IsPressed() => _eventInfo.IsPressed();
    public Vector2 GetPosition() => _eventInfo.GetAdditionalContext();
    
}