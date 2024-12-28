
using Godot;
using NovemberStation.Main;

public class UseGameAction : GameActionBase {
    public UseGameAction(GameAction.Action action) : base(action) { }
    
    [EventListener]
    private void OnUseKeyPress(KeyPressEvent ev, Key key) {
        if (!IsValidKey(key)) return;
        Player player = TestScript.I().GetPlayer();
        
        RaycastResult result = player.GetLookingAt(3.0f);
        
        if (!result.HasHit()) {
            FireEmptyEvent();
            return;
        }

        RaycastResult.HitBodyData firstHitData = result.GetClosestHit();
        Node3D hitObject = firstHitData.Body;

        if (hitObject is RigidBody3D rigidBody3D) {
            ActorPickUpEvent pickUpEvent = new();
            pickUpEvent.SetActor(player);
            pickUpEvent.SetItem(rigidBody3D);
            pickUpEvent.Fire();
            return;
        }
        
        FireEmptyEvent();
    }
    
    private void FireEmptyEvent() {
        ActorPickUpEvent pickUpEvent = new();
        pickUpEvent.SetActor(TestScript.I().GetPlayer());
        pickUpEvent.SetItem(null);
        pickUpEvent.Fire();
    }
}