
using System;
using Godot;

public class UseGameAction : GameActionBase {

    private Type _lastAction;
    public UseGameAction(GameAction.Action action) : base(action) { }

    [EventListener]
    private void OnMouseUsePress(MouseInputEvent ev, Vector2 coords) {
        if (!IsValidInput(ev.GetMouseButton()) || ev.IsCaptured()) return;
        
        Player player = GameManager.I().GetPlayer();
        PlayerController controller = player.GetController<PlayerController>();
        
        Type actionType = controller.GetCurrentContextAction() ?? _lastAction;
        Node3D obj = controller.GetContextObject();

        if (obj == null) return;
        
        IObjectBase objectClass = GameManager.I().GetObjectClass(obj.GetInstanceId());

        if (actionType == null || objectClass == null) return;

        try {
            objectClass.TryGetAction(actionType, out Func<ActorBase, IEventBase, bool> test, out Action<ActorBase, IEventBase> method);
            if (test.Invoke(player, ev)) method.Invoke(player, ev);
            _lastAction = actionType;
        } catch (Exception e) {
            GD.PrintErr($"ERROR: UseGameAction.OnMouseUsePress() : Failed to invoke IObjectAction method '{actionType.GetType().Name}' on object '{objectClass.GetType().Name}'. Exception: {e.Message}");
        }
    }
}