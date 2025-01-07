using System.Collections.Generic;
using System.Linq;
using Godot;

public class Player : ActorBase, IViewable {
    
    private Camera3D _camera;
    
    public Player(CharacterBody3D body) : base(body) {
        new PlayerController(this);

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

    public Camera3D GetCamera() => _camera ??= GetModel().GetNode<Camera3D>("Head/Camera");
    
    public RaycastResult GetLookingAt(float distance) => Raycast.Trace(this, distance);
}