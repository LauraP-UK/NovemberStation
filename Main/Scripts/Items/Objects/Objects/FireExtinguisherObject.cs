
using Godot;

public class FireExtinguisherObject : ObjectBase<RigidBody3D>, IGrabbable {
    public FireExtinguisherObject(RigidBody3D baseNode) : base(baseNode, "extinguisher_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => Items.FIRE_EXTINGUISHER.GetItemName();
    public override string GetContext() => "";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
}