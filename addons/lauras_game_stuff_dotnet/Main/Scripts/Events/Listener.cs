
public abstract class Listener {
    protected Listener() => EventManager.RegisterListeners(this);
    ~Listener() => EventManager.UnregisterListeners(this);
}