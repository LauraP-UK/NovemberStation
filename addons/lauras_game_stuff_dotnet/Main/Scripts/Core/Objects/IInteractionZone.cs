
using Godot;

public interface IInteractionZone : IActionHolder {
    string GetIdentifier();
    Node3D GetContainingNode();
    IObjectBase GetObjectBase();
    string GetDisplayName();
    string GetContext();
    Node3D GetBoundingBoxNode();
}