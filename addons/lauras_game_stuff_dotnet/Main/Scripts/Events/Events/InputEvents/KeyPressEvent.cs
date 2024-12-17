
using Godot;

public class KeyPressEvent : EventBase<Key> {
    private Key _key;

    public KeyPressEvent(Key key) {
        _key = key;
    }

    public override Key GetAdditionalContext() {
        return _key;
    }
}