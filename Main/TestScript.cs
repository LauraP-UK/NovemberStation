

using System;
using System.Collections.Generic;
using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {

    private static TestScript instance;
    private static readonly Scheduler scheduler = new();
    private static InputController inputController;
    
    private Player player;

    private readonly List<RigidBody3D> _cubes = new();

    public TestScript() {
        instance = this;

        EventManager eventManager = new();

        KeyBinding keyBinding = new();
        keyBinding.BindKey(Key.W, GameAction.Action.MOVE_FORWARD);
        keyBinding.BindKey(Key.S, GameAction.Action.MOVE_BACKWARD);
        keyBinding.BindKey(Key.A, GameAction.Action.MOVE_LEFT);
        keyBinding.BindKey(Key.D, GameAction.Action.MOVE_RIGHT);
        keyBinding.BindKey(Key.Space, GameAction.Action.JUMP);
        keyBinding.BindKey(Key.E, GameAction.Action.USE);
        keyBinding.BindKey(Key.Escape, GameAction.Action.QUIT);
        
        inputController = new InputController(keyBinding);
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
            if (child is RigidBody3D cube) _cubes.Add(cube);
        
        GD.Print($"Cubes: {_cubes.Count}");
    }

    private const float DRAW_DISTANCE = 7.0f, DRAW_DISTANCE_SQUARED = DRAW_DISTANCE * DRAW_DISTANCE;
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        MovementActionTracker.Update();

        foreach (RigidBody3D cube in _cubes) {
            if (cube.Position.DistanceSquaredTo(player.GetPosition()) > DRAW_DISTANCE_SQUARED) continue;
            
            foreach (Direction direction in Directions.GetAdjacent()) {
                Vector3 globalDirection = cube.GlobalTransform.Basis * direction.Offset;
                Vector3 endPoint = cube.GlobalPosition + globalDirection;
                
                DebugDraw.Line(cube.GlobalPosition, endPoint, direction.GetAxisDebugColor());
            }
        }
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        player.GetController().Update((float) delta);
    }
    
    public void Quit() => GetTree().Quit();
    public Rid GetWorldRid() => GetTree().Root.GetWorld3D().Space;
}