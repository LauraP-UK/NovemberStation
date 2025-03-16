
using Godot;

public class FireExtinguisherObject : ObjectBase<RigidBody3D>, IGrabbable, ICollectable, IVolumetricObject {
    public FireExtinguisherObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "extinguisher_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<ICollectable>((_,_) => true, (actor,ev) => CollectActionDefault.Invoke(actor, this, ev));
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => Items.FIRE_EXTINGUISHER.GetItemName();
    public override string GetContext() => "";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);
    public float GetSize() => 7.5f;
}