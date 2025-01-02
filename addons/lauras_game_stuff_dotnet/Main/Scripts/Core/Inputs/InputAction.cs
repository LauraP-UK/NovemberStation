using System;
using Godot;

public class InputAction {
    private KeyBinding.InputType Type { get; }
    private Key? Key { get; }
    private MouseButton? MouseButton { get; }

    private InputAction(KeyBinding.InputType type, Key? key = null, MouseButton? mouseButton = null) {
        Type = type;
        Key = key;
        MouseButton = mouseButton;
    }

    public bool Is(Key key) {
        if (Type != KeyBinding.InputType.Key) return false;
        return Key == key;
    }

    public bool Is(MouseButton mouseButton) {
        if (Type != KeyBinding.InputType.MouseButton) return false;
        return MouseButton == mouseButton;
    }

    public static InputAction FromKey(Key key) {
        return new InputAction(KeyBinding.InputType.Key, key: key);
    }

    public static InputAction FromMouseButton(MouseButton mouseButton) {
        return new InputAction(KeyBinding.InputType.MouseButton, mouseButton: mouseButton);
    }

    public override bool Equals(object? obj) {
        if (obj is not InputAction other) return false;
        return Type == other.Type && Key == other.Key && MouseButton == other.MouseButton;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Type, Key, MouseButton);
    }
}