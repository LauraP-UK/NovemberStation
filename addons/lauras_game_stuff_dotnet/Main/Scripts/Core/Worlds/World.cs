using System;
using Godot;

public class World : IWorld {
    private readonly string _name, _filePath;
    private readonly Func<PackedScene, object[], Node3D> _createWorldFunc;
    private readonly bool _backdropWorld;

    private World(string name, string filePath, Func<PackedScene, object[], Node3D> createWorldFunc, bool backdropWorld) {
        _name = name;
        _filePath = filePath;
        _createWorldFunc = createWorldFunc;
        _backdropWorld = backdropWorld;
    }

    public string GetName() => _name;
    public bool IsBackdropWorld() => _backdropWorld;

    public Node3D CreateWorld(object[] args) {
        PackedScene worldScene = Loader.SafeLoad(_filePath);
        if (worldScene == null) throw new InvalidOperationException($"ERROR: World.CreateWorld() : Failed to load world scene from path: {_filePath}");
        return _createWorldFunc(worldScene, args);
    }

    /* --- CONSTRUCTORS --- */

    public static World Create(string name, string filePath, Func<PackedScene, object[], Node3D> createWorldFunc, bool backdropWorld = false) =>
        new(name, filePath, createWorldFunc, backdropWorld);
}