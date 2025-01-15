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
        GameManager.I().SetUILayer();

        Input.MouseMode = Input.MouseModeEnum.Captured;

        Player player = (Player)Characters.PLAYER.CreateActor();
        GetTree().Root.GetNode<Node>("Main/PlayerHolder").AddChild(player.GetModel());
        player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));
        GameManager.I().SetPlayer(player);

        foreach (Node child in GetTree().Root.GetNode<Node3D>("Main/SceneObjects").GetChildren()) {
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

        foreach (RigidBody3D physicsObject in _dynamicObjects.Keys.Where(physicsObject => physicsObject.GlobalPosition.Y < -20)) {
            physicsObject.GlobalPosition = _dynamicObjects[physicsObject];
            //physicsObject.LinearVelocity = Vector3.Zero;
            //physicsObject.AngularVelocity = Vector3.Zero;
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