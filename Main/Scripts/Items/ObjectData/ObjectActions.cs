using NovemberStation.Main.Scripts.Items.ObjectData.Actions;

namespace NovemberStation.Main.Scripts.Items.ObjectData;

public class ObjectActions {
    
    public enum ActionType {
        GRAB,
        SHOVE
    }
    
    public static ActionBase GRAB_ACTION = new GrabAction(ActionType.GRAB, "Grab", 1);
    public static ActionBase SHOVE_ACTION = new ShoveAction(ActionType.SHOVE, "Shove", 2);
    
}