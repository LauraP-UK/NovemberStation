

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {

    private static TestScript instance;
    private static readonly Scheduler scheduler = new();
    private static InputController inputController;
    
    private Player player;

    private readonly Dictionary<RigidBody3D, Vector3> _cubes = new();

    public TestScript() {
        instance = this;

        EventManager eventManager = new();

        KeyBinding.BindInput(Key.W, GameAction.Action.MOVE_FORWARD);
        KeyBinding.BindInput(Key.S, GameAction.Action.MOVE_BACKWARD);
        KeyBinding.BindInput(Key.A, GameAction.Action.MOVE_LEFT);
        KeyBinding.BindInput(Key.D, GameAction.Action.MOVE_RIGHT);
        KeyBinding.BindInput(Key.Space, GameAction.Action.JUMP);
        KeyBinding.BindInput(Key.E, GameAction.Action.USE);
        KeyBinding.BindInput(MouseButton.Left, GameAction.Action.USE);
        KeyBinding.BindInput(Key.Escape, GameAction.Action.QUIT);

        inputController = new InputController();
        GameAction gameAction = new();
    }

    public static TestScript I() => instance;

    public Player GetPlayer() => player;

    public override void _Input(InputEvent @event) => inputController.ProcessInput(@event);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        GD.Print("Start");

        Input.MouseMode = Input.MouseModeEnum.Captured;
        
        player = (Player) Characters.PLAYER.CreateActor();
        GetTree().Root.GetNode<Node>("Main").AddChild(player.GetModel());
        player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));
        
        foreach (Node child in GetTree().Root.GetNode<Node3D>("Main/SceneObjects").GetChildren())
            if (child is RigidBody3D cube) _cubes.Add(cube, cube.GlobalPosition);
        
        GD.Print($"Cubes: {_cubes.Count}");
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        MovementActionTracker.Update();

        if (player.GetModel().Position.Y < -20) player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));
        
        foreach (RigidBody3D physicsObject in _cubes.Keys) {
            float dist = physicsObject.Position.DistanceSquaredTo(player.GetPosition());
            if (dist > 50 * 50) {
                physicsObject.GlobalPosition = _cubes[physicsObject];
                physicsObject.LinearVelocity = Vector3.Zero;
                physicsObject.AngularVelocity = Vector3.Zero;
            }
        }

        RaycastResult raycastResult = Raycast.Trace(player, 3.0f);
        List<RaycastResult.HitBodyData> hitObjs = raycastResult.GetHitsSortedByDistance().Where(obj => obj.Body is RigidBody3D).ToList();

        if (raycastResult.HasHit())
            foreach (RaycastResult.HitBodyData hitObj in hitObjs)
                HighlightObject((RigidBody3D) hitObj.Body);
    }

    private void HighlightObject(RigidBody3D obj) {
        foreach (Direction direction in Directions.GetAdjacent()) {
            Vector3 globalDirection = obj.GlobalTransform.Basis * direction.Offset;
            Vector3 endPoint = obj.GlobalPosition + globalDirection;
                
            DebugDraw.Line(obj.GlobalPosition, endPoint, direction.GetAxisDebugColor());
        }
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        player.GetController().Update((float) delta);
    }
    
    public void Quit() => GetTree().Quit();
    public Rid GetWorldRid() => GetTree().Root.GetWorld3D().Space;
}