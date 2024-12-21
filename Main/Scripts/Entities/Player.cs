using Godot;

public class Player : ActorBase {
    
    public Player() {
        SetName("Player");
        SetPosition(Vector3.Zero);
        new PlayerController(this);
    }
}