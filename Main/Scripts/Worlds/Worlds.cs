using Godot;

public static class Worlds {
    public static readonly World MAIN_WORLD = World.Create(
        "MainWorld",
        "res://Main/Prefabs/Scenes/TestMainScene.tscn",
        (packedScene, args) => Loader.SafeInstantiate<Node3D>(packedScene, true)
    );
    public static readonly World MAIN_BACKDROP = World.Create(
        "MainBackdrop",
        "res://addons/lauras_game_stuff_dotnet/Main/Scenes/Worlds/TODO_BACKDROP.tscn",
        (packedScene, args) => Loader.SafeInstantiate<Node3D>(packedScene, true),
        true
    );
}