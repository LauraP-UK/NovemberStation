

using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {

    private static TestScript instance;
    private static readonly Scheduler scheduler = new();
    private static InputController inputController;

    public TestScript() {
        instance = this;

        KeyBinding keyBinding = new();
        keyBinding.BindKey(Key.W, GameAction.Action.MOVE_FORWARD);
        keyBinding.BindKey(Key.S, GameAction.Action.MOVE_BACKWARD);
        keyBinding.BindKey(Key.A, GameAction.Action.MOVE_LEFT);
        keyBinding.BindKey(Key.D, GameAction.Action.MOVE_RIGHT);
        
        inputController = new InputController(keyBinding, Input.IsKeyPressed, _ => false);
    }

    public static TestScript I() {
        return instance;
    }

    public void PrintSomethingStupid() {
        GD.Print("Start");
    }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        I().PrintSomethingStupid();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        InputController.I().Update();
    }
}