using System;
using Godot;

public class UseGameAction : GameActionBase {

    private ActionKey? _lastAction;
    public UseGameAction(GameAction.Action action) : base(action) { }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnKeyPress(KeyPressEvent ev, Key key) {
        if (!IsValidInput(key) || ev.IsCaptured()) return;
        Handle(ev);
    }
    
    [EventListener(PriorityLevels.TERMINUS)]
    private void OnKeyRelease(KeyReleaseEvent ev, Key key) {
        if (!IsValidInput(key) || ev.IsCaptured()) return;
        Handle(ev);
    }

    [EventListener(PriorityLevels.TERMINUS)]
    private void OnMouseUsePress(MouseInputEvent ev, Vector2 coords) {
        if (!IsValidInput(ev.GetMouseButton()) || ev.IsCaptured()) return;
        Handle(ev);
    }

    private void Handle(IEventBase ev) {
        Player player = GameManager.GetPlayer();
        PlayerController controller = player.GetController<PlayerController>();
        
        ActionKey? actionType = controller.GetCurrentContextAction() ?? _lastAction;
        Node3D obj = controller.GetContextObject();

        if (obj == null) return;
        IObjectBase objectClass = GameManager.GetObjectClass(GameUtils.FindSceneRoot(obj).GetInstanceId());
        if (actionType == null || objectClass == null) return;
        
        IInteractionZone zone = objectClass.FindInteractionZoneFor(obj);

        if (false) {
            Func<ActorBase, IEventBase, bool> test;
            Action<ActorBase, IEventBase> method;
            if (zone == null) {
                objectClass.TryGetAction((ActionKey)actionType, out Func<ActorBase, IEventBase, bool> testOut, out Action<ActorBase, IEventBase> methodOut);
                test = testOut;
                method = methodOut;
            } else {
                zone.TryGetAction((ActionKey)actionType, out Func<ActorBase, IEventBase, bool> testOut, out Action<ActorBase, IEventBase> methodOut);
                test = testOut;
                method = methodOut;
            }
            
            if (test.Invoke(player, ev)) method.Invoke(player, ev);
            _lastAction = actionType;
        }
        else {
            try {
                Func<ActorBase, IEventBase, bool> test;
                Action<ActorBase, IEventBase> method;
                if (zone == null) {
                    objectClass.TryGetAction((ActionKey)actionType, out Func<ActorBase, IEventBase, bool> testOut, out Action<ActorBase, IEventBase> methodOut);
                    test = testOut;
                    method = methodOut;
                } else {
                    zone.TryGetAction((ActionKey)actionType, out Func<ActorBase, IEventBase, bool> testOut, out Action<ActorBase, IEventBase> methodOut);
                    test = testOut;
                    method = methodOut;
                }
            
                if (test.Invoke(player, ev)) method.Invoke(player, ev);
                _lastAction = actionType;
            } catch (Exception e) {
                GD.PrintErr($"ERROR: UseGameAction.OnMouseUsePress() : Failed to invoke IObjectAction method '{actionType.GetType().Name}' on object '{objectClass.GetType().Name}'. Exception: {e.Message}");
            }
        }
    }
}