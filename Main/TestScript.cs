

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
        
        inputController = new InputController(keyBinding);
        GameAction gameAction = new();

        player = new Player();
    }

    public static TestScript I() {
        return instance;
    }
    
    public Player GetPlayer() {
        return player;
    }

    public void PrintSomethingStupid() {
        GD.Print("Start");
    }

    public override void _Input(InputEvent @event) {
        inputController.ProcessInput(@event);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        I().PrintSomethingStupid();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        
    }
}