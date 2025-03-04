using System.Collections.Generic;
using Godot;

public partial class TestScript : Node {
	private readonly Dictionary<RigidBody3D, Vector3> _objSpawns = new();
	
	private readonly SmartDictionary<ulong, IObjectBase> _objects = new();
	private WorldEnvironment _worldEnvironment;
	private EnvironmentType _currentEnvironment = Environments.MORNING;
	private const long DAY_LENGTH = 5000L * 4L;
	
	private long _dayTime = 0L;

	public TestScript() {
		GameManager.Init();
		GameManager.I().SetActiveScene(this);
	}

	public override void _Input(InputEvent @event) => InputController.ProcessInput(@event);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GD.Print("Start");
		
		EventManager.HookWindowResize(GetViewport());
		UIManager.SetUILayer();
		
		GameManager gameManager = GameManager.I();
		gameManager.SetSceneObjects(GetTree().Root.GetNode<Node3D>("Main/SceneObjects"));

		Input.MouseMode = Input.MouseModeEnum.Captured;

		Player player = (Player)Characters.PLAYER.CreateActor();
		GetTree().Root.GetNode<Node>("Main/PlayerHolder").AddChild(player.GetModel());
		player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));
		gameManager.SetPlayer(player);

		Node3D sceneObjects = GetTree().Root.GetNode<Node3D>("Main/SceneObjects");
		foreach (Node child in sceneObjects.GetChildren()) {
			if (child is not Node3D obj) continue;
			gameManager.RegisterObject(obj);
		}

		foreach (Node child in gameManager.GetSceneObjects().GetChildren()) {
			if (child is not RigidBody3D obj) continue;
			_objSpawns.Add(obj, obj.GlobalPosition);
			obj.AngularDamp = 0.5f;
		}

		_worldEnvironment = GetTree().Root.GetNode<WorldEnvironment>("Main/WorldEnvironment");
		//_currentEnvironment.Apply(_worldEnvironment);

		GD.Print($"Dynamic Objects: {_objSpawns.Count}");
		
		Scheduler.ScheduleRepeating(0L, 1000L, _ => _objects.RemoveWhere(pair => GameUtils.IsNodeInvalid(pair.Value.GetBaseNode3D())));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		GameManager gameManager = GameManager.I();
		gameManager.Process(delta);
		
		Player player = gameManager.GetPlayer();
		if (player.GetModel().Position.Y < -20) {
			player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));
			Toast.Warn(player, "You fell off, you numpty. I'm respawning you...");
		}

		if (false && !gameManager.GetTree().IsPaused()) {
			_dayTime += (long)(delta * 1000L);
			if (_dayTime >= DAY_LENGTH) _dayTime = 0L;
			
			EnvironmentType currentEnvironment = _currentEnvironment;
			_currentEnvironment = Mathsf.Remap(0L, DAY_LENGTH, _dayTime, 0, 4) switch {
				0 => Environments.MORNING,
				1 => Environments.DAY,
				2 => Environments.EVENING,
				3 => Environments.NIGHT,
				_ => _currentEnvironment
			};
			if (!currentEnvironment.Equals(_currentEnvironment)) _currentEnvironment.Apply(_worldEnvironment);
		}


		Node sceneObjects = gameManager.GetSceneObjects();
		
		foreach (Node child in sceneObjects.GetChildren()) {
			if (child is not RigidBody3D physicsObj) continue;
			if (!(physicsObj.GlobalPosition.Y < -20)) continue;
			if (_objSpawns.TryGetValue(physicsObj, out Vector3 origPosition))
				physicsObj.GlobalPosition = origPosition;
			else {
				Toast.Error(player, $"Deleted {gameManager.GetObjectClass(physicsObj.GetInstanceId()).GetType()}");
				physicsObj.QueueFree();
			}
		}
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		GameManager.I().PhysicsProcess(delta);
	}
	
	public T GetObjectClass<T>(ulong id) where T : IObjectBase => (T) _objects.GetOrDefault(id, null);
	public SmartDictionary<ulong, IObjectBase> GetObjects() => _objects;
}
