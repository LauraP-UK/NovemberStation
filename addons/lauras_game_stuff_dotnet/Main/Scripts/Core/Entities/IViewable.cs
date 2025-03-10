using Godot;

public interface IViewable {
    
    public Camera3D GetCamera();
    public RaycastResult GetLookingAt(float distance);
    public ActorBase GetActor();
}