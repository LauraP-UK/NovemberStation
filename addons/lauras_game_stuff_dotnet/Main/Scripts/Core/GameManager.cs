using System;
using Godot;

public class GameManager {
    
    private const bool DEBUG_MODE = true;
    public const float SLEEP_ENGINE_SPEED = 30.0f;

    public static readonly float GRAVITY = (float) ProjectSettings.GetSetting("physics/3d/default_gravity");
    
    private static GameManager instance;

    private Node _sceneObjects;
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
        
        GD.Print(
            @"
            ╔═══════════════════════════════════════╗
            ║      Н О Я Б Р Ь   С Т А Н Ц И Я      ║
            ║         — NOVEMBER STATION —          ║
            ║     Awaiting signal... █▒▒▒▒▒▒▒▒▒     ║
            ╚═══════════════════════════════════════╝
        "
        );
        
        EventManager eventManager = new();
        GameAction.Init();
        
        
    }
    public void SetSceneObjects(Node sceneObjects) => _sceneObjects = sceneObjects;
    
    public Node GetSceneObjects() {
        if (_sceneObjects == null) throw new InvalidOperationException("ERROR: GameManager.GetSceneObjects() : Scene objects are null. Set the scene objects first.");
        return _sceneObjects;
    }
    
    public Camera3D GetSleepCamera() => MainLauncher.FindNode<Camera3D>("Main/SleepCamContainer/SleepCam");
    
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
        if (!IsPaused()) {
            if (_player != null) GetPlayer().GetController().Update((float)delta);
            foreach ((ulong _, IObjectBase obj) in MainLauncher.GetGameBootstrapper().GetObjects())
                if (obj is IProcess processObj) processObj.Process((float)delta);
        }

        if (_debugObjects) {
            foreach (IObjectBase obj in MainLauncher.GetGameBootstrapper().GetObjects().Values) {
                CollisionShape3D shape = (CollisionShape3D)obj.GetBaseNode3D().FindChild("BBox");
                if (shape == null) continue;
                BoundingBox bb = BoundingBox.FromCollisionMesh(shape);
                bb.DrawDebugLines(shape.GlobalTransform, Colors.Red);
            }
        }
    }
    
    public void PhysicsProcess(double delta) {
        if (!IsPaused() && _player != null) GetPlayer().GetController().PhysicsUpdate((float)delta);
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

    public SceneTree GetTree() => MainLauncher.GetGameBootstrapper().GetTree();
    public Rid GetWorldRid() => GetTree().Root.GetWorld3D().Space;
    public Viewport GetMasterViewport() => GetTree().Root.GetViewport();
    public Camera3D GetActiveCamera() => MainLauncher.GetGameViewport().GetCamera3D();
    public bool IsActiveCameraPlayer() => GetActiveCamera().Equals(GetPlayer().GetCamera());
    public T GetObjectClass<T>(ulong id) where T : IObjectBase => (T) GetObjectClass(id);
    public IObjectBase GetObjectClass(ulong id)  => MainLauncher.GetGameBootstrapper().GetObjects().GetOrDefault(id, null);

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
        SceneBootstrapper activeScene = MainLauncher.GetGameBootstrapper();
        if (objBase != null) activeScene.GetObjects().Add(rootNode.GetInstanceId(), objBase);
    }

    public void WakeObjects() {
        foreach (IObjectBase obj in MainLauncher.GetGameBootstrapper().GetObjects().Values) {
            Node3D node = obj.GetBaseNode3D();
            if (!GameUtils.IsNodeInvalid(node) && node is RigidBody3D body) body.ApplyImpulse(Vector3.Up * 0.001f);
        }
    }

    public void Pause(bool pause) {
        MainLauncher.GetGameBootstrapper().Pause(pause);
        //MainLauncher.GetGameViewport().GetTree().Paused = pause;
        //GetTree().Paused = pause;
    }
    public bool IsPaused() => MainLauncher.GetGameBootstrapper().IsPaused();

    public void SetMouseControl(bool mouseAvailable) => Input.MouseMode = mouseAvailable ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    public void SetMouseVisible(bool visible) => Input.MouseMode = visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Hidden;
    
    public static bool IsDebugMode() => DEBUG_MODE;

    public void DebugObjects(bool debug) => _debugObjects = debug;
    
    public RaycastResult HighestPoint(Vector3 location, params CollisionObject3D[] ignore) {
        Vector3 start = location + new Vector3(0.0f, 500.0f, 0.0f);
        Vector3 end = location + new Vector3(0.0f, -500.0f, 0.0f);
        return Raycast.Trace(start, end, ignore);
    }
}