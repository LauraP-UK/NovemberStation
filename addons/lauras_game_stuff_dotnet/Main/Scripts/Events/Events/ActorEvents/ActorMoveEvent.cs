using Godot;

public class ActorMoveEvent : ActorEventBase {

    private Vector3 _moveDirection = Vector3.Zero;
    private Vector2 _turnDelta = Vector2.Zero;
    
    public Vector3 GetDirection() => _moveDirection;
    public Vector2 GetTurnDelta() => _turnDelta;
    public void SetDirection(Vector3 direction) => _moveDirection = direction;
    public void SetTurnDelta(Vector2 delta) => _turnDelta = delta;
}