
using Godot;

public class PlayerJumpEvent : ActorMoveEvent {

    private Vector3 _from;
    
    public PlayerJumpEvent() {
        SetActor(GameManager.I().GetPlayer());
    }
    
    public void SetFrom(Vector3 from) => _from = from;
    public Vector3 GetFrom() => _from;

}