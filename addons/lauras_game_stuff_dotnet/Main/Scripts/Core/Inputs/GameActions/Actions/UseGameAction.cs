
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
        Player player = GameManager.I().GetPlayer();
        PlayerController controller = player.GetController<PlayerController>();
        
        ActionKey? actionType = controller.GetCurrentContextAction() ?? _lastAction;
        Node3D obj = controller.GetContextObject();

        if (obj == null) return;
        IObjectBase objectClass = GameManager.I().GetObjectClass(GameUtils.FindSceneRoot(obj).GetInstanceId());
        if (actionType == null || objectClass == null) return;

        if (false) {
            objectClass.TryGetAction((ActionKey)actionType, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> method);
            if (test.Invoke(player, ev)) method.Invoke(player, ev);
            _lastAction = actionType;
        }
        else {
            try {
                objectClass.TryGetAction((ActionKey)actionType, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> method);
                if (test.Invoke(player, ev)) method.Invoke(player, ev);
                _lastAction = actionType;
            } catch (Exception e) {
                GD.PrintErr($"ERROR: UseGameAction.OnMouseUsePress() : Failed to invoke IObjectAction method '{actionType.GetType().Name}' on object '{objectClass.GetType().Name}'. Exception: {e.Message}");
            }
        }
    }
}