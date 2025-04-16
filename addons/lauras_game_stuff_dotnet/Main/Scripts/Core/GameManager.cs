using System;
using Godot;
using Godot.Collections;

public static class GameManager {
    private const bool DEBUG_MODE = true;
    private const string FRAMEWORK_VERSION = "4.4.1";

    public const float SLEEP_ENGINE_SPEED = 30.0f;

    public static readonly float GRAVITY = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    private static readonly string CONSOLE_SPLASH;

    private static Node _sceneObjects;
    private static Player _player;

    private static bool _init, _debugObjects;

    static GameManager() {
        CONSOLE_SPLASH = LoadConsoleSplash();
    }

    public static void Init() {
        if (_init) throw new InvalidOperationException("ERROR: GameManager.Init() : GameManager instance already exists.");
        _init = true;
        GD.Print(CONSOLE_SPLASH);
        GameAction.Init();
    }

    private static string LoadConsoleSplash() {
        string splash;
        Dictionary versionInfo = Engine.GetVersionInfo();
        string version = $"{versionInfo["major"]}.{versionInfo["minor"]}.{versionInfo["patch"]}";
        
        if (ProjectSettings.HasSetting("game/framework_console_splash")) {
            splash = (string)ProjectSettings.GetSetting("game/framework_console_splash");
            splash = splash.Replace("\\n", "\n");
        } else {
            splash =  
                $"        [ WELCOME TO THE PLEASURE DOME ]\n" +
                $"        This is a game framework for Godot {FRAMEWORK_VERSION} (Running: {version})\n" +
                $"        It is designed to be modular and extensible.\n" +
                $"        Please report any bugs or issues to the GitHub repository.\n" +
                $"        Have fun!\n\n";
        }

        return splash;
    }

    public static void SetSceneObjects(Node sceneObjects) => _sceneObjects = sceneObjects;

    public static Node GetSceneObjects() {
        if (_sceneObjects == null) throw new InvalidOperationException("ERROR: GameManager.GetSceneObjects() : Scene objects are null. Set the scene objects first.");
        return _sceneObjects;
    }

    public static Camera3D GetSleepCamera() => MainLauncher.FindNode<Camera3D>("Main/SleepCamContainer/SleepCam");

    public static void SetPlayer(Player player) => _player = player;

    public static Player GetPlayer() {
        if (_player == null) throw new InvalidOperationException("ERROR: GameManager.GetPlayer() : Player is null. Set the player first.");
        return _player;
    }

    /* --- System Methods --- */

    public static void Process(double delta) {
        Scheduler.Process();
        MovementActionTracker.Process();
        UIManager.Process(delta);
        if (!IsPaused()) {
            if (_player != null) GetPlayer().GetController().Update((float)delta);
            foreach ((ulong _, IObjectBase obj) in MainLauncher.GetGameBootstrapper().GetObjects())
                if (obj is IProcess processObj)
                    processObj.Process((float)delta);
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

    public static void PhysicsProcess(double delta) {
        if (!IsPaused() && _player != null) GetPlayer().GetController().PhysicsUpdate((float)delta);
    }

    public static void SetEngineSpeed(float speed) => Engine.SetTimeScale(speed);
    public static void ResetEngineSpeed() => SetEngineSpeed(1.0f);

    /* --- Game Methods --- */

    public static void PopPauseMenu() => new PauseMenu().Open();

    public static void Quit() {
        string leaveMessage = Randf.Random("K thx bye", "Don't leave me!", "I'll miss you!", "Love you!", "Don't go!", "Aww, it was just getting to the good bit!");
        Toast.Info(GetPlayer(), leaveMessage);
        GD.Print(leaveMessage);
        Scheduler.ScheduleOnce(50, _ => GetTree().Quit());
    }

    public static SceneTree GetTree() => MainLauncher.I().GetTree();
    public static Rid GetWorldRid() => GetTree().Root.GetWorld3D().Space;
    public static Viewport GetMasterViewport() => GetTree().Root.GetViewport();
    public static Camera3D GetActiveCamera() => MainLauncher.GetGameViewport().GetCamera3D();
    public static bool IsActiveCameraPlayer() => GetActiveCamera().Equals(GetPlayer().GetCamera());
    public static T GetObjectClass<T>(ulong id) where T : IObjectBase => (T)GetObjectClass(id);
    public static IObjectBase GetObjectClass(ulong id) => MainLauncher.GetGameBootstrapper().GetObjects().GetOrDefault(id, null);

    public static IObjectBase RegisterObject(Node3D node) {
        Node root = GameUtils.FindSceneRoot(node);
        if (root is not Node3D rootNode) {
            GD.PrintErr($"WARN: GameManager.RegisterObject() : Failed to find root Node3D for '{node.Name}'. Got '{root.GetType().Name}' instead.");
            return null;
        }

        IObjectBase objBase = ObjectAtlas.CreateObject(rootNode);
        RegisterObject(rootNode, objBase);
        return objBase;
    }

    public static void RegisterObject(Node3D rootNode, IObjectBase objBase) {
        SceneBootstrapper activeScene = MainLauncher.GetGameBootstrapper();
        if (objBase != null) activeScene.GetObjects().Add(rootNode.GetInstanceId(), objBase);
    }

    public static void WakeObjects() {
        foreach (IObjectBase obj in MainLauncher.GetGameBootstrapper().GetObjects().Values) {
            Node3D node = obj.GetBaseNode3D();
            if (!GameUtils.IsNodeInvalid(node) && node is RigidBody3D body) body.ApplyImpulse(Vector3.Up * 0.001f);
        }
    }

    public static void Pause(bool pause) => MainLauncher.I().GetTree().SetPause(pause);
    public static bool IsPaused() => MainLauncher.I().GetTree().IsPaused();

    public static void SetMouseControl(bool mouseAvailable) => Input.MouseMode = mouseAvailable ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    public static void SetMouseVisible(bool visible) => Input.MouseMode = visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Hidden;

    public static bool IsDebugMode() => DEBUG_MODE;

    public static void DebugObjects(bool debug) => _debugObjects = debug;

    public static RaycastResult HighestPoint(Vector3 location, params CollisionObject3D[] ignore) {
        Vector3 start = new(location.X, 500.0f, location.Z);
        Vector3 end =  new(location.X, -19.5f, location.Z);
        return Raycast.Trace(start, end, ignore);
    }

    public static void SetCanvasLayerFocus(bool focused) {
        MainLauncher.SetMouseFilter(!focused);
    }

    public static void SyncBackdropCamera(Vector3 rotation) => MainLauncher.GetBackdropBootstrapper().SetCameraRotation(rotation);
}