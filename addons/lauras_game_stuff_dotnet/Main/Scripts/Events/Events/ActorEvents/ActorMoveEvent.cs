
using Godot;

public class ActorMoveEvent : ActorEventBase {

    private Vector3 _moveDirection { get; set; }

    public ActorMoveEvent SetDirection(Vector3 direction) {
        _moveDirection = direction;
        return this;
    }
    
    public Vector3 GetDirection() {
        return _moveDirection;
    }
    
}