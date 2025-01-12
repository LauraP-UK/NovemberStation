
using NovemberStation.Main;

public class PlayerMoveEvent : ActorMoveEvent {

    public PlayerMoveEvent() {
        SetActor(GameManager.I().GetPlayer());
    }
    
}