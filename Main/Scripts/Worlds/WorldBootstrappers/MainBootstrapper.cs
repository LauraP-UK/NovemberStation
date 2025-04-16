using System;
using Godot;

public partial class MainBootstrapper : SceneBootstrapper {
    private readonly SmartDictionary<Guid, Vector3> _objSpawns = new();
    private readonly SmartDictionary<ulong, IObjectBase> _objects = new();
    
    protected override void OnPhysicsProcess(double delta) {
        GameManager.PhysicsProcess(delta);
    }
    
    protected override void OnProcess(double delta) {
        GameManager.Process(delta);

        Player player = GameManager.GetPlayer();
        if (player.GetModel().Position.Y < -20) {
            Vector3 spawn = MainLauncher.FindNode<Node3D>("Main/PlayerSpawn").GlobalPosition;
            player.SetPosition(spawn, new Vector3(0.0f, -90.0f, 0.0f));
            Toast.Warn(player, "You fell off, you numpty. I'm respawning you...");
        }

        if (!GameManager.IsPaused()) EnvironmentManager.Process(delta);

        Node sceneObjects = GameManager.GetSceneObjects();

        foreach (Node child in sceneObjects.GetChildren()) {
            if (child is not RigidBody3D physicsObj) continue;
            Vector3 curPos = physicsObj.GlobalPosition;
            if (!(curPos.Y < -20)) continue;
            IObjectBase objClass = GameManager.GetObjectClass(physicsObj.GetInstanceId());
            Vector3 respawnAt = _objSpawns.GetOrDefault(objClass.GetGUID(), default);
            if (respawnAt == default) respawnAt = new Vector3(0, 1, 0);
            RaycastResult highestPoint = GameManager.HighestPoint(curPos, physicsObj);
            if (highestPoint.HasHit()) {
                RaycastResult.HitBodyData hit = highestPoint.GetClosestHit();
                physicsObj.GlobalPosition = hit.HitAtPosition + hit.HitNormal * 0.2f;

                Vector3 up = hit.HitNormal;
                Vector3 forward = physicsObj.GlobalTransform.Basis.Z.Normalized();

                if (Mathf.Abs(up.Dot(forward)) > 0.99f) forward = Vector3.Back;

                Basis surfaceBasis = Basis.LookingAt(forward, up);
                physicsObj.GlobalTransform = new Transform3D(surfaceBasis, physicsObj.GlobalTransform.Origin);
            } else {
                physicsObj.GlobalPosition = respawnAt;
                physicsObj.GlobalRotation = Vector3.Zero;
            }

            physicsObj.LinearVelocity = Vector3.Zero;
        }
    }
    
    protected override void OnReady() {
        EventManager.HookWindowResize(MainLauncher.I().GetTree().Root.GetViewport());
        UIManager.SetUILayer();

        GameManager.SetMouseControl(false);
        GameManager.SetSceneObjects(MainLauncher.FindNode<Node3D>("Main/SceneObjects"));

        Player player = (Player)Characters.PLAYER.CreateActor();
        MainLauncher.FindNode<Node>("Main/PlayerHolder").AddChild(player.GetModel());
        Vector3 spawnPoint = MainLauncher.FindNode<Node3D>("Main/PlayerSpawn").GlobalPosition;
        player.SetPosition(spawnPoint, new Vector3(0.0f, -90.0f, 0.0f));
        GameManager.SetPlayer(player);
        player.AssumeCameraControl();

        foreach (Node child in GameManager.GetSceneObjects().GetChildren()) {
            if (child is not Node3D obj) continue;
            IObjectBase objData = GameManager.RegisterObject(obj);
            if (child is not RigidBody3D rigid) continue;
            _objSpawns.Add(objData.GetGUID(), obj.GlobalPosition);
            rigid.AngularDamp = 0.5f;
        }

        Node3D sunObj = Loader.SafeInstantiate<Node3D>("res://Main/Prefabs/Sandbox/Sun.tscn");
        AddChild(sunObj);

        EnvironmentManager.Init(
            MainLauncher.FindNode<WorldEnvironment>("Main/WorldEnvironment"),
            MainLauncher.FindNode<Node3D>("Main/SunContainer"),
            sunObj,
            MainLauncher.FindNode<DirectionalLight3D>("Main/SunContainer/Sun")
        );

        GD.Print($"Dynamic Objects: {_objSpawns.Count}");

        Scheduler.ScheduleRepeating(0L, 1000L, _ => _objects.RemoveWhere(pair => GameUtils.IsNodeInvalid(pair.Value.GetBaseNode3D())));
    }

    public override SmartDictionary<ulong, IObjectBase> GetObjects() => _objects;
    public override T GetObjectClass<T>(ulong id) => (T)_objects.GetOrDefault(id, null);
}