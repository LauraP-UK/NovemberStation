
using Godot;

public class GrabActionDefault : BaseActionDefault {
    private static void Handle(IViewable actor, Node3D node, bool pickUp) {
        RaycastResult result = actor.GetLookingAt(3.0f);
        
        if (!pickUp || !result.HasHit()) {
            FireEmptyEvent();
            return;
        }

        RaycastResult.HitBodyData hitData = result.GetViaBody(node);
        if (hitData == null) {
            GD.PrintErr($"ERROR: GrabActionDefault.Handle() : hitData is null for {node.Name}");
            FireEmptyEvent();
            return;
        }
        Vector3 hitNormal = hitData.HitNormal;
        Vector3 hitAtPosition = hitData.HitAtPosition;

        if (node is not RigidBody3D rigidBody3D) {
            FireEmptyEvent();
            return;
        }
        
        ActorPickUpEvent pickUpEvent = new();
        pickUpEvent.SetActor(actor.GetActor());
        pickUpEvent.SetItem(rigidBody3D);
        pickUpEvent.SetInteractAt(hitAtPosition);
        pickUpEvent.SetInteractNormal(hitNormal);
        pickUpEvent.Fire();
    }
    
    private static void FireEmptyEvent() {
        ActorPickUpEvent pickUpEvent = new();
        pickUpEvent.SetActor(GameManager.I().GetPlayer());
        pickUpEvent.SetItem(null);
        pickUpEvent.Fire();
    }

    public static void Invoke(ActorBase actorBase, Node3D node, IEventBase ev) {
        if (actorBase is not IViewable actor) return;
        switch (ev) {
            case MouseInputEvent mouseEv: {
                Handle(actor, node, mouseEv.IsPressed());
                break;
            }
            default:
                return;
        }
    }
}