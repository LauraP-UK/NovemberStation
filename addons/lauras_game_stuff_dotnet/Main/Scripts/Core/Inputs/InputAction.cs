using System;
using Godot;

public class InputAction {
    private enum InputType { KEY, MOUSE_BUTTON }
    private InputType Type { get; }
    private Key? Key { get; }
    private MouseButton? MouseButton { get; }

    private InputAction(InputType type, Key? key = null, MouseButton? mouseButton = null) {
        Type = type;
        Key = key;
        MouseButton = mouseButton;
    }

    public bool Is(Key key) {
        if (Type != InputType.KEY) return false;
        return Key == key;
    }

    public bool Is(MouseButton mouseButton) {
        if (Type != InputType.MOUSE_BUTTON) return false;
        return MouseButton == mouseButton;
    }

    public static InputAction FromKey(Key key) {
        return new InputAction(InputType.KEY, key: key);
    }

    public static InputAction FromMouseButton(MouseButton mouseButton) {
        return new InputAction(InputType.MOUSE_BUTTON, mouseButton: mouseButton);
    }

    public override bool Equals(object? obj) {
        if (obj is not InputAction other) return false;
        return Type == other.Type && Key == other.Key && MouseButton == other.MouseButton;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Type, Key, MouseButton);
    }
}