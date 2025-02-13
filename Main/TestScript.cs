using System.Collections.Generic;
using Godot;

public partial class TestScript : Node {
	private readonly Dictionary<RigidBody3D, Vector3> _objSpawns = new();
	
	private readonly SmartDictionary<ulong, IObjectBase> _objects = new();

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

		GD.Print($"Dynamic Objects: {_objSpawns.Count}");
		
		Scheduler.ScheduleRepeating(0L, 1000L, _ => {
			List<ulong> toRemove = new();
			foreach ((ulong instanceID, IObjectBase obj) in _objects) {
				Node3D node3D = obj.GetBaseNode3D();
				if (node3D == null || !IsInstanceValid(node3D) || node3D.IsQueuedForDeletion()) toRemove.Add(instanceID);
			}
			_objects.RemoveAll(toRemove);
		});
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		GameManager.I().Process(delta);
		
		Player player = GameManager.I().GetPlayer();
		if (player.GetModel().Position.Y < -20) player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));

		Node sceneObjects = GameManager.I().GetSceneObjects();
		
		foreach (Node child in sceneObjects.GetChildren()) {
			if (child is not RigidBody3D physicsObj) continue;
			if (!(physicsObj.GlobalPosition.Y < -20)) continue;
			if (_objSpawns.TryGetValue(physicsObj, out Vector3 origPosition))
				physicsObj.GlobalPosition = origPosition;
			else
				physicsObj.QueueFree();
		}
	}

	public override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		GameManager.I().PhysicsProcess(delta);
	}
	
	public T GetObjectClass<T>(ulong id) where T : IObjectBase => (T) _objects.GetOrDefault(id, null);
	public SmartDictionary<ulong, IObjectBase> GetObjects() => _objects;
}
