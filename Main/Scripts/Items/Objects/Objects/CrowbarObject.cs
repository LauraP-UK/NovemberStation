
using Godot;

public class CrowbarObject : ObjectBase<RigidBody3D>, IGrabbable, ICollectable, IVolumetricObject {

    public CrowbarObject(RigidBody3D baseNode) : base(baseNode, "crowbar_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<ICollectable>((_,_) => true, (actor,ev) => CollectActionDefault.Invoke(actor, this, ev));
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => Items.CROWBAR.GetItemName();
    public override string GetContext() => "Every good scientist needs a crowbar!";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);
    public float GetSize() => 5.0f;
}