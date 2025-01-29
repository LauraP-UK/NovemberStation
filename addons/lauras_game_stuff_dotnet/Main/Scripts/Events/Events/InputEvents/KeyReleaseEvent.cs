
using Godot;

public class KeyReleaseEvent : EventBase<Key> {
    private readonly Key _key;
    private bool _captured;
    
    public KeyReleaseEvent(Key key) => _key = key;
    public override Key GetAdditionalContext() => _key;
    public void Capture() => _captured = true;
    public bool IsCaptured() => _captured;
}