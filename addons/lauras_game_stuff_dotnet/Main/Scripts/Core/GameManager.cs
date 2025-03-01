
using System;
using Godot;

public class GameManager {
    
    private static GameManager instance;

    private Node _activeScene, _sceneObjects;
    private Player _player;
    
    private GameManager() => instance = this;

    public static GameManager I() {
        if (instance == null) throw new InvalidOperationException("ERROR: GameManager.I() : GameManager instance is null. Did you forget to call GameManager.Init()?");
        return instance;
    }
    
    public static void Init() {
        if (instance != null) throw new InvalidOperationException("ERROR: GameManager.Init() : GameManager instance already exists.");
        new GameManager();
        
        EventManager eventManager = new();
        GameAction.Init();
    }
    
    public void SetActiveScene(Node scene) => _activeScene = scene;
    public void SetSceneObjects(Node sceneObjects) => _sceneObjects = sceneObjects;

    public Node GetActiveScene() {
        if (_activeScene == null) throw new InvalidOperationException("ERROR: GameManager.GetActiveScene() : Active scene is null. Set the active scene first.");
        return _activeScene;
    }
    
    public Node GetSceneObjects() {
        if (_sceneObjects == null) throw new InvalidOperationException("ERROR: GameManager.GetSceneObjects() : Scene objects are null. Set the scene objects first.");
        return _sceneObjects;
    }
    
    public void SetPlayer(Player player) => _player = player;
    public Player GetPlayer() {
        if (_player == null) throw new InvalidOperationException("ERROR: GameManager.GetPlayer() : Player is null. Set the player first.");
        return _player;
    }
    
    /* --- System Methods --- */
    
    public void Process(double delta) {
        Scheduler.Process();
        MovementActionTracker.Process();
        UIManager.Process(delta);
        if (!GetTree().Paused) GetPlayer().GetController().Update((float)delta);
    }
    
    public void PhysicsProcess(double delta) {
        if (!GetTree().Paused) GetPlayer().GetController().PhysicsUpdate((float)delta);
    }

    /* --- Game Methods --- */
    
    public void PopPauseMenu() => new PauseMenu().Open();

    public void Quit() {
        Toast.Info(GetPlayer(), "K thx bye");
        Scheduler.ScheduleOnce(50, _ => GetTree().Quit());
    }

    public SceneTree GetTree() => GetActiveScene().GetTree();
    public Rid GetWorldRid() => GetTree().Root.GetWorld3D().Space;
    public Viewport GetViewport() => GetTree().Root.GetViewport();
    public Camera3D GetActiveCamera() => GetViewport().GetCamera3D();
    public bool IsActiveCameraPlayer() => GetActiveCamera().Equals(GetPlayer().GetCamera());
    public T GetObjectClass<T>(ulong id) where T : IObjectBase => (T) GetObjectClass(id);
    public IObjectBase GetObjectClass(ulong id)  => ((TestScript)GetActiveScene()).GetObjects().GetOrDefault(id, null);

    public void RegisterObject(Node3D node) {
        Node root = GameUtils.FindSceneRoot(node);
        if (root is not Node3D rootNode) {
            GD.PrintErr($"WARN: GameManager.RegisterObject() : Failed to find root Node3D for '{node.Name}'. Got '{root.GetType().Name}' instead.");
            return;
        }
        TestScript activeScene = (TestScript)GetActiveScene();
        IObjectBase objBase = ObjectAtlas.CreateObject(rootNode);
        if (objBase != null)
            activeScene.GetObjects().Add(rootNode.GetInstanceId(), objBase);
    }
    
    public void Pause(bool pause) => GetTree().Paused = pause;
    public void SetMouseControl(bool mouseAvailable) => Input.MouseMode = mouseAvailable ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    public void SetMouseVisible(bool visible) => Input.MouseMode = visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Hidden;
}