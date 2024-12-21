public abstract class ActorEventBase : CancellableEventBase<ActorBase> {

    private ActorBase _actor { get; set; }
    
    public ActorEventBase SetActor(ActorBase actor) {
        _actor = actor;
        return this;
    }
    
    public override ActorBase GetAdditionalContext() {
        return _actor;
    }
}