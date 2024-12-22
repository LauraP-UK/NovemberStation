using Godot;

public class Player : ActorBase {
    
    private Camera3D _camera;
    
    public Player(CharacterBody3D body) : base(body) {
        new PlayerController(this);
    }

    public Camera3D GetCamera() => _camera ??= GetModel().GetNode<Camera3D>("Camera");
}