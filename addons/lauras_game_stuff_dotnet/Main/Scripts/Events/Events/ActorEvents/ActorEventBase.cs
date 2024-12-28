public abstract class ActorEventBase : CancellableEventBase<ActorBase> {

    private ActorBase _actor { get; set; }
    
    public void SetActor(ActorBase actor) => _actor = actor;
    public override ActorBase GetAdditionalContext() => _actor;
}