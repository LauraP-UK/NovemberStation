
using Godot;

public class UseAction : ActionBase {
    
    public UseAction(ObjectActions.ActionType actionType, string actionName, int index) : base(actionType, actionName, index) { }
    public override void Invoke<T>(ActorBase actorBase, T node, ObjectData objectData) {
        GD.Print($"Actor {actorBase.GetName()} is using {node.Name}...");
    }
}