using Godot;

public interface IViewable {
    public Camera3D GetCamera();
    public void AssumeCameraControl();
    public RaycastResult GetLookingAt(float distance);
    public ActorBase GetActor();
    public float GetLookSmoothness();
}