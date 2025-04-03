using System;
using Godot;

public static class Loader {
    private static readonly SmartDictionary<string, PackedScene> _cache = new();
    public static PackedScene SafeLoad(string path, bool throwError = false) {
        if (_cache.TryGetValue(path.ToLowerInvariant(), out PackedScene load)) return load;
        if (!ResourceLoader.Exists(path)) {
            GD.PrintErr($"ERROR: Loader.SafeLoad() : Resource not found: {path}");
            if (throwError) throw new InvalidOperationException($"ERROR: Loader.SafeLoad() : Resource not found: {path}");
            return null;
        }
        PackedScene packedScene = GD.Load<PackedScene>(path);
        if (packedScene == null) {
            GD.PrintErr($"ERROR: Loader.SafeLoad() : Failed to load PackedScene: {path}");
            if (throwError) throw new InvalidOperationException($"ERROR: Loader.SafeLoad() : Failed to load PackedScene: {path}");
            return null;
        }

        _cache.Add(path.ToLowerInvariant(), packedScene);
        return packedScene;
    }
    public static T SafeInstantiate<T>(string path, bool throwError = false) where T : Node {
        PackedScene packedScene = SafeLoad(path, throwError);
        if (packedScene != null) {
            Node instance = packedScene.Instantiate();
            if (instance is T typedInstance) return typedInstance;
            GD.PrintErr($"ERROR: Loader.SafeInstantiate() : PackedScene at path '{path}' is not of type {typeof(T)}.");
            if (throwError) throw new InvalidCastException($"ERROR: Loader.SafeInstantiate() : PackedScene at path '{path}' is not of type {typeof(T)}.");
            return null;
        }
        GD.PrintErr($"ERROR: Loader.SafeInstantiate() : PackedScene not found: {path}");
        if (throwError) throw new InvalidOperationException($"ERROR: Loader.SafeInstantiate() : PackedScene not found: {path}");
        return null;
    }
    public static void ClearCache() => _cache.Clear();
}