
using Godot;

public class ActorPickUpEvent : ActorEventBase {
    
    private RigidBody3D _item;
    private Vector3 _interactAt, _iteractNormal;
    
    public RigidBody3D GetItem() => _item;
    public Vector3 GetInteractAt() => _interactAt;
    public Vector3 GetInteractNormal() => _iteractNormal;
    public void SetItem(RigidBody3D item) => _item = item;
    public void SetInteractAt(Vector3 interactAt) => _interactAt = interactAt;
    public void SetInteractNormal(Vector3 interactNormal) => _iteractNormal = interactNormal;
}