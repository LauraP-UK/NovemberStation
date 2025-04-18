using System.Collections.Generic;
using System.Linq;
using Godot;

public static class MovementActionTracker {

    private static readonly List<MovementGameAction> _movementActions = new();
    private static JumpGameAction _jumpAction;
    
    public static void RegisterMovementAction(MovementGameAction movementAction) {
        if (movementAction is JumpGameAction jumpAction) {
            _jumpAction = jumpAction;
            return;
        }
        _movementActions.Add(movementAction);
    }

    public static void Process() {
        Vector3 movement = _movementActions.Where(action => action.IsKeyPressed()).Aggregate(Vector3.Zero, (current, action) => current + action.GetOffset());
        if (!movement.Equals(Vector3.Zero)) {
            movement = movement.Normalized();
            PlayerMoveEvent moveEvent = new();
            moveEvent.SetDirection(movement);
            moveEvent.Fire();
        }
        
        if (_jumpAction != null && _jumpAction.IsKeyPressed()) {
            PlayerJumpEvent jumpEvent = new();
            jumpEvent.SetFrom(GameManager.GetPlayer().GetPosition());
            jumpEvent.Fire();
        }
    }
}