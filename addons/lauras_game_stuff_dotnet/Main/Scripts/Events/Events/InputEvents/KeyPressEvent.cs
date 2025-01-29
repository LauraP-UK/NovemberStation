
using Godot;

public class KeyPressEvent : EventBase<Key>, ICapturable {
    private readonly Key _key;
    private bool _captured;
    
    public KeyPressEvent(Key key) => _key = key;
    public override Key GetAdditionalContext() => _key;
    public void Capture() => _captured = true;
    public bool IsCaptured() => _captured;
}