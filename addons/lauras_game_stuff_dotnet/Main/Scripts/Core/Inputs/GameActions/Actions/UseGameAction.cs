
using Godot;
using NovemberStation.Main;

public class UseGameAction : GameActionBase {
    public UseGameAction(GameAction.Action action) : base(action) { }

    [EventListener]
    private void OnMouseUsePress(MouseClickEvent ev, Vector2 coords) {
        if (!IsValidInput(ev.GetMouseButton())) return;
        new PlayerUseClickEvent(ev.GetMouseButton(), ev.IsPressed(), coords).Fire();
    }
    
    [EventListener]
    private void OnUseKeyPress(KeyPressEvent ev, Key key) {
        if (!IsValidInput(key)) return;
        Player player = GameManager.I().GetPlayer();
        
        RaycastResult result = player.GetLookingAt(3.0f);
        
        if (!result.HasHit()) {
            FireEmptyEvent();
            return;
        }

        RaycastResult.HitBodyData firstHitData = result.GetClosestHit();
        Node3D hitObject = firstHitData.Body;
        Vector3 hitNormal = firstHitData.HitNormal;
        Vector3 hitAtPosition = firstHitData.HitAtPosition;

        if (hitObject is RigidBody3D rigidBody3D) {
            ActorPickUpEvent pickUpEvent = new();
            pickUpEvent.SetActor(player);
            pickUpEvent.SetItem(rigidBody3D);
            pickUpEvent.SetInteractAt(hitAtPosition);
            pickUpEvent.SetInteractNormal(hitNormal);
            pickUpEvent.Fire();
            return;
        }
        
        FireEmptyEvent();
    }
    
    private void FireEmptyEvent() {
        ActorPickUpEvent pickUpEvent = new();
        pickUpEvent.SetActor(GameManager.I().GetPlayer());
        pickUpEvent.SetItem(null);
        pickUpEvent.Fire();
    }
}