

using System;
using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {

    private static TestScript instance;
    private static readonly Scheduler scheduler = new();
    private static InputController inputController;
    
    private Player player;

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
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) => MovementActionTracker.Update();

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        player.GetController().Update((float) delta);
    }
    
    public void Quit() => GetTree().Quit();
    public Rid GetRid() => GetTree().Root.GetWorld3D().Space;
}