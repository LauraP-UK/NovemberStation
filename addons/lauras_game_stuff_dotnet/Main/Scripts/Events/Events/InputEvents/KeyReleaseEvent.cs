
using Godot;

public class KeyReleaseEvent : EventBase<Key> {
    
    private Key _key;
    public KeyReleaseEvent(Key key) => _key = key;
    public override Key GetAdditionalContext() => _key;
}