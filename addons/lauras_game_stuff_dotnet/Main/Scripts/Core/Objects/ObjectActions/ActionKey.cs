using System;

public struct ActionKey {
    public readonly Type InterfaceType;
    public readonly string CustomName;

    public bool IsCustom => CustomName != null;

    public ActionKey(Type interfaceType) {
        InterfaceType = interfaceType;
        CustomName = null;
    }

    public ActionKey(string customName) {
        CustomName = customName;
        InterfaceType = null;
    }

    public override bool Equals(object obj) {
        return obj is ActionKey other &&
               InterfaceType == other.InterfaceType &&
               CustomName == other.CustomName;
    }

    public override int GetHashCode() {
        unchecked {
            return ((InterfaceType?.GetHashCode() ?? 0) * 397) ^ (CustomName?.GetHashCode() ?? 0);
        }
    }

    public override string ToString() => IsCustom ? CustomName : InterfaceType?.Name ?? "Unknown";
}