public abstract class ActorEventBase : EventBase<string> {

    private string ActorName { get; }

    protected ActorEventBase(string actorName) {
        ActorName = actorName;
    }
    
    public override string GetAdditionalContext() {
        return ActorName;
    }
}