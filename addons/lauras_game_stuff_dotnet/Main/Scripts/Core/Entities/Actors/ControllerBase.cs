
public abstract class ControllerBase : Listener {
    
    private ActorBase _actor { get; set; }
    
    protected ControllerBase(ActorBase actor) {
        _actor = actor;
        _actor.SetController(this);
    }
    
    protected ActorBase GetActor() {
        return _actor;
    }
    
}