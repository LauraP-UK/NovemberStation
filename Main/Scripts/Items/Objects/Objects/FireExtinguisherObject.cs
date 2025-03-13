
using System;
using Godot;

public class FireExtinguisherObject : ObjectBase<RigidBody3D>, IGrabbable {
    public FireExtinguisherObject(RigidBody3D baseNode) : base(baseNode, "extinguisher_obj", "extinguisher_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => "Fire Extinguisher";
    public override string GetContext() => "";
    public override SmartDictionary<string, (object, Action<object>)> GetSerializeData() => new();
}