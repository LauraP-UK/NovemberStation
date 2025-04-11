using System;
using Godot;

public static class GameUtils {
    
    private static readonly Cache<Node, Node> _sceneRootNodeCache = new(() => null);

    public static string FindSceneFilePath(Node node) {
        Node current = node;
        while (current != null) {
            if (current.SceneFilePath != "")
                return current.SceneFilePath;
            current = current.GetParent();
        }
        return "ERR";
    }
    
    public static Node FindSceneRoot(Node node) {
        Node cachedRoot = _sceneRootNodeCache.GetFromCache(node);
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

    public static bool IsNodeInvalid(Node node) => node == null || !GodotObject.IsInstanceValid(node) || node.IsQueuedForDeletion();
}