using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

public static class GameAction {
    public enum Action {
        MOVE_FORWARD,
        MOVE_BACKWARD,
        MOVE_LEFT,
        MOVE_RIGHT,
        JUMP,
        CROUCH,
        TURN_CAMERA,
        USE,
        QUIT,
        NONE
    }
    
    public static readonly GameActionBase MOVE_FORWARD = new MovementGameAction(Action.MOVE_FORWARD, Vector3.Forward);
    public static readonly GameActionBase MOVE_BACKWARD = new MovementGameAction(Action.MOVE_BACKWARD, Vector3.Back);
    public static readonly GameActionBase MOVE_LEFT = new MovementGameAction(Action.MOVE_LEFT, Vector3.Left);
    public static readonly GameActionBase MOVE_RIGHT = new MovementGameAction(Action.MOVE_RIGHT, Vector3.Right);
    public static readonly GameActionBase JUMP = new JumpGameAction(Action.JUMP);
    public static readonly GameActionBase CROUCH = new CrouchGameAction(Action.CROUCH);
    public static readonly GameActionBase TURN_CAMERA = new TurnCameraGameAction(Action.TURN_CAMERA);
    public static readonly GameActionBase USE = new UseGameAction(Action.USE);
    public static readonly GameActionBase QUIT = new QuitGameAction(Action.QUIT);
    public static readonly GameActionBase NONE =new NoOpGameAction(Action.NONE);

    private static readonly GameActionBase[] _all;
    static GameAction() {
        _all = typeof(GameAction)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => typeof(GameActionBase).IsAssignableFrom(field.FieldType))
            .Select(field => field.GetValue(null) as GameActionBase)
            .Where(p => p != null)
            .ToArray();
        GD.Print("[GameAction] INFO: Registered all game actions.");
    }
    
    public static void Init() => GetAll(); // Dummy method to ensure static constructor is called

    public static List<GameActionBase> GetAll() => new(_all);
    public static GameActionBase GetDefault() => NONE;
}