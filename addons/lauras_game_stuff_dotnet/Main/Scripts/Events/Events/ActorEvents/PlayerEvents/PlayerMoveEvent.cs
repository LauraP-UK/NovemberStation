
using NovemberStation.Main;

public class PlayerMoveEvent : ActorMoveEvent {

    public PlayerMoveEvent() {
        SetActor(TestScript.I().GetPlayer());
    }
    
}