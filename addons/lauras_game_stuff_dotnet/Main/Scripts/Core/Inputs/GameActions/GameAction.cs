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
    
    public static readonly GameActionSpecific MOVE_FORWARD = GameActionSpecific.Create(Action.MOVE_FORWARD);
    public static readonly GameActionSpecific MOVE_BACKWARD = GameActionSpecific.Create(Action.MOVE_BACKWARD);
    public static readonly GameActionSpecific MOVE_LEFT = GameActionSpecific.Create(Action.MOVE_LEFT);
    public static readonly GameActionSpecific MOVE_RIGHT = GameActionSpecific.Create(Action.MOVE_RIGHT);
    public static readonly GameActionSpecific JUMP = GameActionSpecific.Create(Action.JUMP);
    public static readonly GameActionSpecific USE = GameActionSpecific.Create(Action.USE);
    public static readonly GameActionSpecific NONE = GameActionSpecific.Create(Action.NONE);

    public static List<GameActionSpecific> GetAll() {
        List<GameActionSpecific> returnList = new() {
            MOVE_FORWARD,
            MOVE_BACKWARD,
            MOVE_LEFT,
            MOVE_RIGHT,
            JUMP,
            USE
        };
        return returnList;
    }
    
    public static GameActionSpecific GetDefault() {
        return NONE;
    }
    
}