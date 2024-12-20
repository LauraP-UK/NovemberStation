
using Godot;

public class ActorMoveEvent : ActorEventBase {

    private Vector3 _dummyLocation { get; set; }
    
    public ActorMoveEvent() {
        
    }
    
    public ActorMoveEvent SetDummyLocation(Vector3 dummyLocation) {
        _dummyLocation = dummyLocation;
        return this;
    }
    
}