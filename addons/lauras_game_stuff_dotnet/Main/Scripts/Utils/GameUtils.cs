using System;
using Godot;

public static class GameUtils {
    
    private static readonly Cache<Node, Node>
        _sceneRootNodeCache = new(() => null),
        _taggedParentCache = new(() => null);
    private static readonly Cache<Node, string> 
        _sceneFilePathCache = new(() => "");

    public static string FindSceneFilePath(Node node) {
        string cachedPath = _sceneFilePathCache.GetFromCache(node, false);
        if (!string.IsNullOrEmpty(cachedPath)) return cachedPath;
        Node current = node;
        while (current != null) {
            if (current.SceneFilePath != "") {
                _sceneFilePathCache.AddToCache(node, current.SceneFilePath);
                return current.SceneFilePath;
            }
            current = current.GetParent();
        }
        return "";
    }
    
    public static Node FindSceneRoot(Node node) {
        Node cachedRoot = _sceneRootNodeCache.GetFromCache(node, false);
        if (cachedRoot != null) return cachedRoot;
        Node current = node;
        while (current != null) {
            if (current.SceneFilePath != "") {
                _sceneRootNodeCache.AddToCache(node, current);
                return current;                
            }
            Node parent = current.GetParent();
            if (parent == null) return current;
            current = parent;
        }
        return null;
    }
    
    public static T FindSceneRoot<T>(Node node) where T : Node {
        Node rootNode = FindSceneRoot(node);
        if (rootNode is not T typedNode) throw new InvalidOperationException($"ERROR: GameUtils.FindSceneRoot<{typeof(T)}> : Root node is not of type {typeof(T)}. Got '{rootNode?.GetType()}'.");
        return typedNode;
    }

    public static Node GetParentTaggedNode(Node node) {
        Node tagged = _taggedParentCache.GetFromCache(node, false);
        if (tagged != null) return tagged;
        Node current = node;
        while (current != null) {
            if (current.HasMeta(ObjectAtlas.OBJECT_TAG)) {
                _taggedParentCache.AddToCache(node, current);
                return current;
            }
            current = current.GetParent();
        }
        return null;
    }

    public static SmartSet<Node> GetAllChildren(Node parent) {
        SmartSet<Node> allChildren = [];
        AddChildrenRecursive(parent, allChildren);
        return allChildren;
    }
    
    private static void AddChildrenRecursive(Node parent, SmartSet<Node> collection) {
        foreach (Node child in parent.GetChildren()) {
            collection.Add(child);
            AddChildrenRecursive(child, collection);
        }
    }

    public static bool IsNodeInvalid(Node node) => node == null || !GodotObject.IsInstanceValid(node) || node.IsQueuedForDeletion();
}