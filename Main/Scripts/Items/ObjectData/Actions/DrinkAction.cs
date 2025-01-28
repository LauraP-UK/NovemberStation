
using Godot;

public class DrinkAction : ActionBase {
    
    public DrinkAction(ObjectActions.ActionType actionType, string actionName, int index) : base(actionType, actionName, index) { }
    public override void Invoke<T>(ActorBase actorBase, T node, ObjectData objectData) {
        GD.Print($"Actor {actorBase.GetName()} is drinking from {node.Name}...");
    }
}