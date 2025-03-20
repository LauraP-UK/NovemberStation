using Godot;

public class BedObject : ObjectBase<RigidBody3D>, IGrabbable {
    public BedObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "bed_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterArbitraryAction("Sleep", 10, (_, _) => true, Sleep);
    }

    private void Sleep(ActorBase actor, IEventBase eventBase) {
        if (eventBase is not KeyPressEvent) return;
        ((Player)actor).GetController<PlayerController>().Sleep();
    }

    public override string GetDisplayName() => Items.BED.GetItemName();
    public override string GetContext() => "";
    public override string GetSummary() => "";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
}