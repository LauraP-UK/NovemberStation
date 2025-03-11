
using Godot;

public class CubeObject : ObjectBase<RigidBody3D>, IGrabbable, IShovable {
    public CubeObject(RigidBody3D baseNode) : base(baseNode, "cube_obj", "cube_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<IShovable>((_,_) => true, Shove);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Shove(ActorBase actorBase, IEventBase ev) => ShoveActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => "Storage Crate";
}