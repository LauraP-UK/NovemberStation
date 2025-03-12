
using Godot;

public class CrowbarObject : ObjectBase<RigidBody3D>, IGrabbable {

    public CrowbarObject(RigidBody3D baseNode) : base(baseNode, "crowbar_obj", "crowbar_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => "Crowbar";
    public override string GetContext() => "Every good scientist needs a crowbar!";
}