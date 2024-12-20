
public abstract class EventBase<TContext> {
    public abstract TContext GetAdditionalContext();

    public virtual void Fire() {
        EventManager.I().FireEvent(this);
    }
}
