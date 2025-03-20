using Godot;

public class DeskObject : ObjectBase<RigidBody3D>, IGrabbable {
    public DeskObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "desk_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_,_) => true, Grab);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => Items.WORK_DESK.GetItemName();
    public override string GetContext() => "";
    public override string GetSummary() => "";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
}