using System.Collections.Generic;
using System.Linq;
using Godot;

public class Player : ActorBase, IViewable {
    
    private readonly Camera3D _camera;
    private readonly Node3D _crouchNode;
    
    public Player(CharacterBody3D body) : base(body) {
        new PlayerController(this);
        _crouchNode = GetModel().GetNode<Node3D>("HeadOrigin/CrouchControl");
        _camera = _crouchNode.GetNode<Camera3D>("Camera");

        List<VisualInstance3D> children = GetVisualModel()
            .GetChildren()
            .OfType<VisualInstance3D>()
            .ToList();

        foreach (VisualInstance3D child in children) {
            child.SetLayerMaskValue(1, false);
            child.SetLayerMaskValue(2, true);
        }
        
        GetCamera().SetCullMaskValue(2, false);
    }

    public Camera3D GetCamera() => _camera;
    public Node3D GetCrouchNode() => _crouchNode;
    
    public RaycastResult GetLookingAt(float distance) => Raycast.Trace(this, distance);
    public ActorBase GetActor() => this;
}