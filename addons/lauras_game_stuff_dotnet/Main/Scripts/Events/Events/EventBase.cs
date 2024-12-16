
public abstract class EventBase<T> {
    public abstract T GetAdditionalContext();

    public virtual void Fire() {
        EventManager.I().FireEvent<EventBase<T>, T>(this);
    }
}
