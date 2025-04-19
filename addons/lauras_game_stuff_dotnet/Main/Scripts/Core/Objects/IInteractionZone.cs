
using Godot;

public interface IInteractionZone : IActionHolder {
    string GetIdentifier();
    Node3D GetContainingNode();
    IObjectBase GetObjectBase();
    string GetDisplayName();
    string GetContext();
    bool IsActive();
    Node3D GetBoundingBoxNode();
}