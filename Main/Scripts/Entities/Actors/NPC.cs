using Godot;

public class NPC : ActorBase {
    
    public NPC(CharacterBody3D body) : base(body) {
        new NPCController(this);
    }
}