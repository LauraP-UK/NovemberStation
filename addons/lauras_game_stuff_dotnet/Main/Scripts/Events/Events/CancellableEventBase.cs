
public abstract class CancellableEventBase<TContext> : EventBase<TContext>, ICancellable {
    
    private bool _canceled;
    public bool IsCanceled() => _canceled;
    
    public void SetCanceled(bool canceled) {
        _canceled = canceled;
    }
}