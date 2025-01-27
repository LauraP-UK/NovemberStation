
using Godot;

public class GrabAction : ActionBase {
    public GrabAction(ObjectActions.ActionType actionType, string name, int index) : base(actionType, name, index) { }
    private static void Handle(IViewable actor) {
        RaycastResult result = actor.GetLookingAt(3.0f);
        
        if (!result.HasHit()) {
            FireEmptyEvent();
            return;
        }

        RaycastResult.HitBodyData firstHitData = result.GetClosestHit();
        Node3D hitObject = firstHitData.Body;
        Vector3 hitNormal = firstHitData.HitNormal;
        Vector3 hitAtPosition = firstHitData.HitAtPosition;

        if (hitObject is not RigidBody3D rigidBody3D) {
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

    public override void Invoke<T>(ActorBase actorBase, T node, ObjectData objectData) {
        if (actorBase is not IViewable actor) return;
        Handle(actor);
    }
}