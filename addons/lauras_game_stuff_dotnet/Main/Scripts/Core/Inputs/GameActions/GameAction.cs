using System;
using System.Collections.Generic;
using Godot;

public class GameAction {
    public enum Action {
        MOVE_FORWARD,
        MOVE_BACKWARD,
        MOVE_LEFT,
        MOVE_RIGHT,
        JUMP,
        TURN_CAMERA,
        USE,
        QUIT,
        NONE
    }
    
    private static GameAction _instance;
    private static MovementActionTracker _movementActionTracker = new();
    
    public static readonly GameActionBase MOVE_FORWARD = new MovementGameAction(Action.MOVE_FORWARD, Vector3.Forward);
    public static readonly GameActionBase MOVE_BACKWARD = new MovementGameAction(Action.MOVE_BACKWARD, Vector3.Back);
    public static readonly GameActionBase MOVE_LEFT = new MovementGameAction(Action.MOVE_LEFT, Vector3.Left);
    public static readonly GameActionBase MOVE_RIGHT = new MovementGameAction(Action.MOVE_RIGHT, Vector3.Right);
    public static readonly GameActionBase JUMP = new MovementGameAction(Action.JUMP, new Vector3(0.0f, 10.0f, 0.0f));
    public static readonly GameActionBase TURN_CAMERA = new TurnCameraGameAction(Action.TURN_CAMERA);
    public static readonly GameActionBase USE = new UseGameAction(Action.USE);
    public static readonly GameActionBase QUIT = new QuitGameAction(Action.QUIT);
    public static readonly GameActionBase NONE =new NoOpGameAction(Action.NONE);

    private static readonly List<GameActionBase> _all = new() {
        MOVE_FORWARD,
        MOVE_BACKWARD,
        MOVE_LEFT,
        MOVE_RIGHT,
        TURN_CAMERA,
        JUMP,
        USE,
        QUIT
    };
    
    public GameAction() {
        if (_instance != null) throw new InvalidOperationException("ERROR: GameAction.<init> : GameAction is a singleton and cannot be instantiated more than once.");
        _instance = this;
    }
    
    public static GameAction I() {
        if (_instance == null) throw new InvalidOperationException("ERROR: GameAction.GetInstance : GameAction has not been instantiated yet.");
        return _instance;
    }

    public static List<GameActionBase> GetAll() => new(_all);

    public static GameActionBase GetDefault() => NONE;
}