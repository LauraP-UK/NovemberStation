
using Godot;

public class KeyPressEvent : EventBase<Key> {
    private readonly Key _key;
    public KeyPressEvent(Key key) => _key = key;
    public override Key GetAdditionalContext() => _key;
}