
public class KeyReleaseEvent : EventBase<string> {
    
    private string _key;
    
    public KeyReleaseEvent(string key) {
        _key = key;
    }
    
    public override string GetAdditionalContext() {
        return _key;
    }
}