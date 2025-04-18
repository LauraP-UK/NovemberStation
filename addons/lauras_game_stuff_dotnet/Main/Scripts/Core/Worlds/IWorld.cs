using Godot;

public interface IWorld {
    public string GetName();
    public bool IsBackdropWorld();
    public Node3D CreateWorld(object[] args);
}