using System;
using Godot;

public static class Loader {
    private static readonly Cache<string, PackedScene> _cache = new(() => null, -1);
    public static PackedScene SafeLoad(string path, bool throwError = false) {
        string pathLower = path.ToLowerInvariant();
        PackedScene fromCache = _cache.GetFromCache(pathLower, false);
        if (fromCache != null) return fromCache;
        
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

        _cache.AddToCache(pathLower, packedScene);
        return packedScene;
    }

    public static T SafeInstantiate<T>(string path, bool throwError = false) where T : Node {
        PackedScene packedScene = SafeLoad(path, throwError);
        if (packedScene != null) return SafeInstantiate<T>(packedScene, throwError);
        if (throwError) throw new InvalidOperationException($"ERROR: Loader.SafeInstantiate() : PackedScene not found: {path}");
        GD.PrintErr($"ERROR: Loader.SafeInstantiate() : PackedScene not found: {path}");
        return null;
    }

    public static T SafeInstantiate<T>(PackedScene packedScene, bool throwError = false) where T : Node {
        Node instance = packedScene?.Instantiate();
        if (instance is T typedInstance) return typedInstance;
        if (throwError) throw new InvalidOperationException($"ERROR: Loader.SafeInstantiate() : PackedScene not found!");
        GD.PrintErr($"ERROR: Loader.SafeInstantiate() : PackedScene not found!");
        return null;
    }
    public static void ClearCache() => _cache.ClearCache();
}