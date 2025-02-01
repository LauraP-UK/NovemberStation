
using Godot;

public static class ObjectDataAtlas {
    private const string META_TAG = "behaviour_type";
    
    private static readonly SmartDictionary<string, ObjectData> _registry = new();
    private static ObjectData Register(string metaTag) {
        ObjectData data = new(metaTag);
        _registry.Add(data.GetMetaTag(), data);
        return data;
    }

    public static ObjectData Get(string metaTag) => _registry.GetOrDefault(metaTag, null);
    public static ObjectData Get(Node3D node) => !HasBehaviour(node) ? null : Get(node.GetMeta(META_TAG).AsString());
    public static bool HasBehaviour(Node3D node) => node.HasMeta(META_TAG);
}