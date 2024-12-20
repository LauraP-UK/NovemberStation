public abstract class ActorEventBase : EventBase<string> {

    private string _actorName { get; set; }

    protected ActorEventBase() {
    }
    
    public ActorEventBase SetActorName(string actorName) {
        _actorName = actorName;
        return this;
    }
    
    public override string GetAdditionalContext() {
        return _actorName;
    }
}