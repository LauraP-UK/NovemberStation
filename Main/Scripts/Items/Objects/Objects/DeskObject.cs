﻿using Godot;

public class DeskObject : ObjectBase<RigidBody3D>, IGrabbable {
    public DeskObject(RigidBody3D baseNode) : base(baseNode, "desk_obj", "desk_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
}