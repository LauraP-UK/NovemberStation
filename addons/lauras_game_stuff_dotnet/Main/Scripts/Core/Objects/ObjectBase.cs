
using Godot;

public abstract class ObjectBase<T> : IObjectBase where T : Node3D {
    private T _baseNode;
    protected ObjectBase(T baseNode) => _baseNode = baseNode;
    protected TType FindNode<TType>(string nodePath) where TType : Node => _baseNode.GetNode<TType>(nodePath);
    public static string GetObjectTag() => "base_object";
}