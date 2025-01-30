
using Godot;

public class UseAction : ActionBase {
    public UseAction(ObjectActions.ActionType actionType, string actionName, int index) : base(actionType, actionName, index) { }
    public override void Invoke<T>(ActorBase actorBase, T node, IEventBase ev) {
        if (!CanRun(ev)) return;
        Node rootNode = GameUtils.FindSceneRoot(node);
        PCObject pcObject = GameManager.I().GetObjectClass<PCObject>(rootNode.GetInstanceId());
        pcObject.View();
    }
    protected override MouseType GetMouseType() => MouseType.DOWN;
}