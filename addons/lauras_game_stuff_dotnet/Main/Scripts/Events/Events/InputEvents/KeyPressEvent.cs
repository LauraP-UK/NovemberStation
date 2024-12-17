
public class KeyPressEvent : EventBase<string> {
    private string _key;

    public KeyPressEvent(string key) {
        _key = key;
    }

    public override string GetAdditionalContext() {
        return _key;
    }
}