using System;
using Godot;

public partial class MainLauncher : Control {
    private const string
        MAIN_VIEWPORT_PATH = "ScreenManager/World/SubViewport",
        BACKDROP_VIEWPORT_PATH = "ScreenManager/Backdrop/SubViewport",
        MAIN_PATH = "ScreenManager",
        BACKDROP_PATH = "ScreenManager/Backdrop",
        WORLD_PATH = "ScreenManager/World";

    // This is the main entry point for the game. It initializes the game manager and creates the main world.
    private static MainLauncher _instance;

    private SubViewport _mainWorldViewport, _backdropViewport;
    private SceneBootstrapper _world;
    private BackdropBootstrapper _backdrop;

    private Control _mainControl, _backdropControl, _worldControl;

    public MainLauncher() {
        if (_instance != null) throw new InvalidOperationException("ERROR: MainLauncher.<init> : MainLauncher instance already exists!");
        _instance = this;
        GameManager.Init();
    }
    
    [EventListener]
    private void OnWindowResized(WindowResizeEvent ev, Vector2 size) => SetViewportSize(size);

    public override void _Input(InputEvent ev) => InputController.ProcessInput(ev);

    public override void _Ready() {
        _mainControl = GetTree().Root.GetNode<Control>(MAIN_PATH);
        _backdropControl = GetTree().Root.GetNode<SubViewportContainer>(BACKDROP_PATH);
        _worldControl = GetTree().Root.GetNode<SubViewportContainer>(WORLD_PATH);
        
        _backdropViewport = GetTree().Root.GetNode<SubViewport>(BACKDROP_VIEWPORT_PATH);
        _mainWorldViewport = GetTree().Root.GetNode<SubViewport>(MAIN_VIEWPORT_PATH);

        Node3D mainBackdrop = Worlds.MAIN_BACKDROP.CreateWorld(null);
        _backdrop = mainBackdrop as BackdropBootstrapper;
        _backdropViewport.AddChild(mainBackdrop);
        
        Node3D mainWorld = Worlds.MAIN_WORLD.CreateWorld(null);
        _world = mainWorld as SceneBootstrapper;
        _mainWorldViewport.AddChild(mainWorld);
        
        EventManager.RegisterListeners(this);
        SetViewportSize(GetTree().Root.GetViewport().GetVisibleRect().Size);
    }
    
    /* --- Static Methods --- */
    
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

    public static SceneBootstrapper GetGameBootstrapper() => I()._world;
    public static BackdropBootstrapper GetBackdropBootstrapper() => I()._backdrop;

    public static void SetMouseFilter(bool ignore) {
        MainLauncher launcher = I();
        launcher._mainControl.SetMouseFilter(ignore ? MouseFilterEnum.Ignore : MouseFilterEnum.Stop);
        launcher._backdropControl.SetMouseFilter(ignore ? MouseFilterEnum.Ignore : MouseFilterEnum.Pass);
        launcher._worldControl.SetMouseFilter(ignore ? MouseFilterEnum.Ignore : MouseFilterEnum.Pass);
    }

    private static void SetViewportSize(Vector2 size) {
        Vector2I viewportSize = new((int)size.X, (int)size.Y);
        I()._mainWorldViewport.SetSize(viewportSize);
        I()._backdropViewport.SetSize(viewportSize);
    }
}