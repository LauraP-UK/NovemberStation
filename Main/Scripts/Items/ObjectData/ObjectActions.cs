
public static class ObjectActions {
    
    public enum ActionType {
        USE,
        GRAB,
        SHOVE,
        DRINK
    }
    
    public static ActionBase GRAB_ACTION = new GrabAction(ActionType.GRAB, "Grab", 1);
    public static ActionBase USE_ACTION = new UseAction(ActionType.USE, "Use", 2);
    public static ActionBase SHOVE_ACTION = new ShoveAction(ActionType.SHOVE, "Shove", 3);
    public static ActionBase DRINK_ACTION = new DrinkAction(ActionType.DRINK, "Drink", 4);

    private static ActionBase[] _actions = {
        GRAB_ACTION,
        USE_ACTION,
        SHOVE_ACTION,
        DRINK_ACTION
    };
}