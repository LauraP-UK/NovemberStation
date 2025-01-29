using Godot;

public class ShoveAction : ActionBase {
    public ShoveAction(ObjectActions.ActionType actionType, string actionName, int index) : base(actionType, actionName, index) { }
    public override void Invoke<T>(ActorBase actorBase, T node, IEventBase ev) {
        if (!CanRun(ev)) return;
        if (node is not RigidBody3D rigidBody3D) return;
        if (actorBase is not Player player) return;
        Vector3 tossDirection = -player.GetCamera().GlobalTransform.Basis.Z * 5.0f;
        rigidBody3D.LinearVelocity += tossDirection;
    }
    protected override MouseType GetMouseType() => MouseType.DOWN;
}