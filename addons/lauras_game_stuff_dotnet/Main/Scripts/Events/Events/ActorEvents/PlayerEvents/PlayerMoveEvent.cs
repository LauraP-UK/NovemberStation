

public class PlayerMoveEvent : ActorMoveEvent {

    public PlayerMoveEvent() {
        SetActor(GameManager.GetPlayer());
    }
    
}