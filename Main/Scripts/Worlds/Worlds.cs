using Godot;

public static class Worlds {
    public static readonly World MAIN_WORLD = World.Create(
        "MainWorld",
        "res://Main/Prefabs/Scenes/TestMainScene.tscn",
        (packedScene, args) => Loader.SafeInstantiate<Node3D>(packedScene, true)
    );
    public static readonly World MAIN_BACKDROP = World.Create(
        "MainBackdrop",
        "res://Main/Prefabs/Scenes/TestBackdrop.tscn",
        (packedScene, args) => Loader.SafeInstantiate<Node3D>(packedScene, true),
        true
    );
}