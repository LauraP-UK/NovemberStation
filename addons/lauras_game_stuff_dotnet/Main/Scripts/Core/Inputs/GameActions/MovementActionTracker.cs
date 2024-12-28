using System.Collections.Generic;
using System.Linq;
using Godot;

public class MovementActionTracker {

    private static readonly List<MovementGameAction> _movementActions = new();
    
    public static void RegisterMovementAction(MovementGameAction movementAction) {
        _movementActions.Add(movementAction);
    }

    public static void Update() {
        Vector3 movement = _movementActions.Where(action => action.IsKeyPressed()).Aggregate(Vector3.Zero, (current, action) => current + action.GetOffset());
        if (movement.Equals(Vector3.Zero)) return;
        movement = movement.Normalized();
        PlayerMoveEvent moveEvent = new();
        moveEvent.SetDirection(movement);
        moveEvent.Fire();
    }
}