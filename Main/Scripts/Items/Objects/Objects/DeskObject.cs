using System;
using Godot;

public class DeskObject : ObjectBase<RigidBody3D>, IGrabbable {
    public DeskObject(RigidBody3D baseNode) : base(baseNode, "desk_obj", "desk_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => "Desk";
    public override string GetContext() => "";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
}