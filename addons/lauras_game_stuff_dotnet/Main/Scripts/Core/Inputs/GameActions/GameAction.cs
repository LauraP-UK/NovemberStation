using System;
using System.Collections.Generic;

public class GameAction {
    public enum Action {
        MOVE_FORWARD,
        MOVE_BACKWARD,
        MOVE_LEFT,
        MOVE_RIGHT,
        JUMP,
        USE,
        NONE
    }
    
    public static readonly GameActionBase MOVE_FORWARD = new MovementGameAction(Action.MOVE_FORWARD);
    public static readonly GameActionBase MOVE_BACKWARD = new MovementGameAction(Action.MOVE_BACKWARD);
    public static readonly GameActionBase MOVE_LEFT = new MovementGameAction(Action.MOVE_LEFT);
    public static readonly GameActionBase MOVE_RIGHT = new MovementGameAction(Action.MOVE_RIGHT);
    public static readonly GameActionBase JUMP = new JumpGameAction(Action.JUMP);
    public static readonly GameActionBase USE = new UseGameAction(Action.USE);
    public static readonly GameActionBase NONE =new NoOpGameAction(Action.NONE);

    private static readonly List<GameActionBase> _all = new() {
        MOVE_FORWARD,
        MOVE_BACKWARD,
        MOVE_LEFT,
        MOVE_RIGHT,
        JUMP,
        USE
    };

    public static List<GameActionBase> GetAll() {
        return new List<GameActionBase>(_all);
    }
    
    public static GameActionBase GetDefault() {
        return NONE;
    }
    
}