
using System;
using Godot;

public class GameManager {
    
    private static readonly Scheduler scheduler = new();
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

    /* --- Game Methods --- */
    
    public void PopPauseMenu() => new PauseMenu().Open();

    public void Quit() => Scheduler.ScheduleOnce(50, _ => GetActiveScene().GetTree().Quit());

    public Rid GetWorldRid() => GetActiveScene().GetTree().Root.GetWorld3D().Space;
    public T GetObjectClass<T>(ulong id) where T : IObjectBase => (T) ((TestScript)GetActiveScene()).GetObjects().GetOrDefault(id, null);

    public void RegisterObject(Node3D node) {
        TestScript activeScene = (TestScript)GetActiveScene();
        IObjectBase objBase = ObjectAtlas.CreateObject(node);
        if (objBase != null)
            activeScene.GetObjects().Add(node.GetInstanceId(), objBase);
    }
    
    public void Pause(bool pause) => GetActiveScene().GetTree().Paused = pause;
}