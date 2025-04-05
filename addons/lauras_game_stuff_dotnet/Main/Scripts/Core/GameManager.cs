using System;
using Godot;

public class GameManager {
    
    private const bool DEBUG_MODE = true;
    public const float SLEEP_SPEED = 20.0f;
    
    private static GameManager instance;

    private Node _activeScene, _sceneObjects;
    private Player _player;
    
    private bool _debugObjects;
    
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
        ObjectSerialiserCache.GetFromCache("dummy", "call");
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
    
    public Camera3D GetSleepCamera() => GetTree().Root.GetNode<Camera3D>("Main/SleepCamContainer/SleepCam");
    
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
        if (!GetTree().Paused) {
            if (_player != null) GetPlayer().GetController().Update((float)delta);
            foreach ((ulong _, IObjectBase obj) in ((TestScript)GetActiveScene()).GetObjects())
                if (obj is IProcess processObj) processObj.Process((float)delta);
        }

        if (_debugObjects) {
            foreach (IObjectBase obj in ((TestScript)GetActiveScene()).GetObjects().Values) {
                CollisionShape3D shape = (CollisionShape3D)obj.GetBaseNode3D().FindChild("BBox");
                if (shape == null) continue;
                BoundingBox bb = BoundingBox.FromCollisionMesh(shape);
                bb.DrawDebugLines(shape.GlobalTransform, Colors.Yellow);
            }
        }
    }
    
    public void PhysicsProcess(double delta) {
        if (!GetTree().Paused && _player != null) GetPlayer().GetController().PhysicsUpdate((float)delta);
    }

    public static void SetEngineSpeed(float speed) => Engine.SetTimeScale(speed);
    public static void ResetEngineSpeed() => SetEngineSpeed(1.0f);

    /* --- Game Methods --- */
    
    public void PopPauseMenu() => new PauseMenu().Open();

    public void Quit() {
        string leaveMessage = Randf.Random("K thx bye", "Don't leave me!", "I'll miss you!", "Love you!", "Don't go!", "Aww, it was just getting to the good bit!");
        Toast.Info(GetPlayer(), leaveMessage);
        GD.Print(leaveMessage);
        Scheduler.ScheduleOnce(50, _ => GetTree().Quit());
    }

    public SceneTree GetTree() => GetActiveScene().GetTree();
    public Rid GetWorldRid() => GetTree().Root.GetWorld3D().Space;
    public Viewport GetViewport() => GetTree().Root.GetViewport();
    public Camera3D GetActiveCamera() => GetViewport().GetCamera3D();
    public bool IsActiveCameraPlayer() => GetActiveCamera().Equals(GetPlayer().GetCamera());
    public T GetObjectClass<T>(ulong id) where T : IObjectBase => (T) GetObjectClass(id);
    public IObjectBase GetObjectClass(ulong id)  => ((TestScript)GetActiveScene()).GetObjects().GetOrDefault(id, null);

    public IObjectBase RegisterObject(Node3D node) {
        Node root = GameUtils.FindSceneRoot(node);
        if (root is not Node3D rootNode) {
            GD.PrintErr($"WARN: GameManager.RegisterObject() : Failed to find root Node3D for '{node.Name}'. Got '{root.GetType().Name}' instead.");
            return null;
        }
        IObjectBase objBase = ObjectAtlas.CreateObject(rootNode);
        RegisterObject(rootNode, objBase);
        return objBase;
    }
    public void RegisterObject(Node3D rootNode, IObjectBase objBase) {
        TestScript activeScene = (TestScript)GetActiveScene();
        if (objBase != null) activeScene.GetObjects().Add(rootNode.GetInstanceId(), objBase);
    }

    public void WakeObjects() {
        foreach (IObjectBase obj in ((TestScript)GetActiveScene()).GetObjects().Values) {
            Node3D node = obj.GetBaseNode3D();
            if (!GameUtils.IsNodeInvalid(node) && node is RigidBody3D body) body.ApplyImpulse(Vector3.Up * 0.001f);
        }
    }
    
    public void Pause(bool pause) => GetTree().Paused = pause;
    public void SetMouseControl(bool mouseAvailable) => Input.MouseMode = mouseAvailable ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    public void SetMouseVisible(bool visible) => Input.MouseMode = visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Hidden;
    
    public static bool IsDebugMode() => DEBUG_MODE;

    public void DebugObjects(bool debug) => _debugObjects = debug;
}