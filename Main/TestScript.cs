using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {
    private readonly Dictionary<RigidBody3D, Vector3> _dynamicObjects = new();

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

        foreach (Node child in gameManager.GetSceneObjects().GetChildren()) {
            if (child is not RigidBody3D obj) continue;
            _dynamicObjects.Add(obj, obj.GlobalPosition);
            obj.AngularDamp = 0.5f;
        }

        GD.Print($"Dynamic Objects: {_dynamicObjects.Count}");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        MovementActionTracker.Update();
        Player player = GameManager.I().GetPlayer();

        if (player.GetModel().Position.Y < -20) player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));

        Node sceneObjects = GameManager.I().GetSceneObjects();
        
        foreach (Node child in sceneObjects.GetChildren()) {
            if (child is not RigidBody3D physicsObj) continue;
            if (!(physicsObj.GlobalPosition.Y < -20)) continue;
            if (_dynamicObjects.TryGetValue(physicsObj, out Vector3 origPosition)) {
                physicsObj.GlobalPosition = origPosition;
                //physicsObj.LinearVelocity = Vector3.Zero;
                //physicsObj.AngularVelocity = Vector3.Zero;
            }
            else
                physicsObj.QueueFree();
        }

        RaycastResult raycastResult = Raycast.Trace(player, 3.0f);
        List<RaycastResult.HitBodyData> hitObjs = raycastResult.GetHitsSortedByDistance().Where(obj => obj.Body is RigidBody3D).ToList();

        if (!raycastResult.HasHit()) return;
        foreach (RaycastResult.HitBodyData hitObj in hitObjs)
            HighlightObject((RigidBody3D)hitObj.Body);
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        Player player = GameManager.I().GetPlayer();
        if (!GetTree().Paused) player.GetController().Update((float)delta);
    }

    private static void HighlightObject(RigidBody3D obj) {
        foreach (Direction direction in Directions.GetAdjacent()) {
            Vector3 globalDirection = obj.GlobalTransform.Basis * direction.Offset;
            Vector3 endPoint = obj.GlobalPosition + globalDirection;

            DebugDraw.Line(obj.GlobalPosition, endPoint, direction.GetAxisDebugColor());
        }
    }
}