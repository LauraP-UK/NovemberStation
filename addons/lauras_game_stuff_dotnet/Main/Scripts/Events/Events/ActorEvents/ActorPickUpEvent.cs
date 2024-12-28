
using Godot;

public class ActorPickUpEvent : ActorEventBase {
    
    private RigidBody3D _item;
    
    public RigidBody3D GetItem() => _item;
    public void SetItem(RigidBody3D item) => _item = item;
    
}