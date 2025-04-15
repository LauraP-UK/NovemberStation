using System;
using Godot;

public partial class MainLauncher : Node {
    private const string
        MAIN_VIEWPORT_PATH = "ScreenManager/World/SubViewport",
        BACKDROP_VIEWPORT_PATH = "ScreenManager/Backdrop/SubViewport";

    // This is the main entry point for the game. It initializes the game manager and creates the main world.
    private static MainLauncher _instance;

    private SubViewport _mainWorldViewport, _backdropViewport;
    private SceneBootstrapper _main, _backdrop;

    public MainLauncher() {
        if (_instance != null) throw new InvalidOperationException("ERROR: MainLauncher.<init> : MainLauncher instance already exists!");
        _instance = this;
        GameManager.Init();
    }

    public override void _Input(InputEvent ev) => InputController.ProcessInput(ev);

    public override void _Ready() {
        _backdropViewport = GetTree().Root.GetNode<SubViewport>(BACKDROP_VIEWPORT_PATH);
        _mainWorldViewport = GetTree().Root.GetNode<SubViewport>(MAIN_VIEWPORT_PATH);

        Node3D mainWorld = Worlds.MAIN_WORLD.CreateWorld(null);
        _main = mainWorld as SceneBootstrapper;
        _mainWorldViewport.AddChild(mainWorld);
    }
    public static MainLauncher I() => _instance;

    public static Viewport GetGameViewport() {
        return I()._mainWorldViewport;
    }
    public static T FindNode<T>(string path, bool backdrop = false) where T : Node {
        SceneTree tree = _instance.GetTree();
        T node = tree.Root.GetNode<T>((backdrop ? BACKDROP_VIEWPORT_PATH : MAIN_VIEWPORT_PATH) + "/" + path);
        if (node != null) return node;
        GD.PrintErr($"ERROR: MainLauncher.FindNode() : Node not found at path: {path}");
        return null;
    }

    public static SceneBootstrapper GetGameBootstrapper() => I()._main;
}