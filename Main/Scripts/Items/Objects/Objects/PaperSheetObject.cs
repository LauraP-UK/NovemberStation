
using Godot;

public class PaperSheetObject : ObjectBase<RigidBody3D>, IGrabbable, ICollectable, IWritable, IVolumetricObject {

    public PaperSheetObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "papersheet_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<ICollectable>((_,_) => true, (actor,ev) => CollectActionDefault.Invoke(actor, this, ev));
        RegisterAction<IWritable>((_,_) => true, OpenWritingUI);
    }
    public override string GetDisplayName() => Items.PAPER_SHEET.GetItemName();
    public override string GetContext() => "";
    public override string GetSummary() => "";

    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);
    public float GetSize() => 0.05f;
    public void OpenWritingUI(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent && ev is not MouseInputEvent) return;
        if (actorBase is not IHotbarActor hbActor || ev is MouseInputEvent && !IsHeldItem(hbActor)) return;
        Toast.Info(GameManager.I().GetPlayer(), "TODO: Open writing UI");
    }
}