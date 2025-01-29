
using Godot;

public class UseGameAction : GameActionBase {
    private bool _downLastFrame = false;
    public UseGameAction(GameAction.Action action) : base(action) { }

    [EventListener]
    private void OnMouseUsePress(MouseInputEvent ev, Vector2 coords) {
        if (!IsValidInput(ev.GetMouseButton())) return;
        new PlayerUseClickEvent(ev.GetMouseButton(), ev.IsPressed(), coords).Fire();
    }
    
    [EventListener]
    private void OnUseKeyPress(KeyPressEvent ev, Key key) {
        if (!IsValidInput(key)) return;
        if (_downLastFrame) return;
        
        _downLastFrame = true;
        Player player = GameManager.I().GetPlayer();
        
        RaycastResult result = player.GetLookingAt(3.0f);
        
        if (!result.HasHit()) return;

        RaycastResult.HitBodyData firstHitData = result.GetClosestHit();
        Node3D hitObject = firstHitData.Body;
        Vector3 hitNormal = firstHitData.HitNormal;
        Vector3 hitAtPosition = firstHitData.HitAtPosition;

        if (hitObject is not RigidBody3D rigidBody3D) return;
        ActorPickUpEvent pickUpEvent = new();
        pickUpEvent.SetActor(player);
        pickUpEvent.SetItem(rigidBody3D);
        pickUpEvent.SetInteractAt(hitAtPosition);
        pickUpEvent.SetInteractNormal(hitNormal);
        pickUpEvent.Fire();
    }
    
    [EventListener]
    private void OnKeyRelease(KeyReleaseEvent ev, Key key) {
        if (!IsValidInput(key)) return;
        FireEmptyEvent();
        _downLastFrame = false;
    }
    
    private void FireEmptyEvent() {
        ActorPickUpEvent pickUpEvent = new();
        pickUpEvent.SetActor(GameManager.I().GetPlayer());
        pickUpEvent.SetItem(null);
        pickUpEvent.Fire();
    }
}