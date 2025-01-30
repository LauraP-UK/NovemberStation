
using Godot;

public class DrinkAction : ActionBase {
    public DrinkAction(ObjectActions.ActionType actionType, string actionName, int index) : base(actionType, actionName, index) { }
    public override void Invoke<T>(ActorBase actorBase, T node, IEventBase ev) {
        if (!CanRun(ev)) return;
        Node rootNode = GameUtils.FindSceneRoot(node);
        GasCanObject gasCanObject = GameManager.I().GetObjectClass<GasCanObject>(rootNode.GetInstanceId());
        gasCanObject.DrinkFrom();
    }
    protected override MouseType GetMouseType() => MouseType.DOWN;
}