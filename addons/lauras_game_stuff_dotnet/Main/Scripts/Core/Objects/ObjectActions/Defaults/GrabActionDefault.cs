
using Godot;

public class GrabActionDefault : BaseActionDefault {
    private const ulong RUN_COOLDOWN = 1000;
    
    private static ulong _lastRan;
    private static bool _isHeld;

    private static void Handle(IViewable actor, Node3D node, bool pickUp) {
        RaycastResult result = Raycast.TraceActive(3.0f);
        
        if (!pickUp || !result.HasHit()) {
            FireEmptyEvent();
            return;
        }

        Node root = GameUtils.FindSceneRoot(node);
        RaycastResult.HitBodyData hitData = result.GetViaRoot(root as Node3D);
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
        _isHeld = true;
        _lastRan = Time.GetTicksMsec();
    }
    
    private static void FireEmptyEvent() {
        ActorPickUpEvent pickUpEvent = new();
        pickUpEvent.SetActor(GameManager.GetPlayer());
        pickUpEvent.SetItem(null);
        pickUpEvent.Fire();
        _isHeld = false;
    }

    public static void Invoke(ActorBase actorBase, Node3D node, IEventBase ev) {
        if (actorBase is not IViewable actor) return;
        switch (ev) {
            case MouseInputEvent mouseEv: {
                if (!mouseEv.IsPressed() || !_isHeld) return;
                FireEmptyEvent();
                _lastRan = Time.GetTicksMsec();
                ShoveActionDefault.Invoke(actorBase, node, ev, 20.0f, MouseType.DOWN, KeyType.NONE);
                break;
            }
            case KeyPressEvent keyPress: {
                if (_isHeld) return;
                if (Time.GetTicksMsec() - _lastRan < RUN_COOLDOWN) return;
                Handle(actor, node, true);
                break;
            }
            case KeyReleaseEvent keyRelease: {
                _lastRan = 0;
                Handle(actor, node, false);
                break;
            }
            default:
                return;
        }
    }
}