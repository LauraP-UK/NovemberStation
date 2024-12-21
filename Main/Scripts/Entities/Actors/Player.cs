using Godot;

public class Player : ActorBase {
    
    public Player(CharacterBody3D body) : base(body) {
        new PlayerController(this);
    }
}