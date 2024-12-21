
using Godot;

public interface IActor : IEntity {
    string GetName();
    Vector3 GetPosition();
    void SetPosition(Vector3 position);
}