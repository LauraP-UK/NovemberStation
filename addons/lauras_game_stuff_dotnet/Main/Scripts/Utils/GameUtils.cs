
using Godot;

public static class GameUtils {

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
        Node current = node;
        while (current != null) {
            if (current.SceneFilePath != "")
                return current;
            current = current.GetParent();
        }
        return null;
    }
    
}