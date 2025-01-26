using Godot;

namespace NovemberStation.Main.Scripts.Items.ObjectData.Actions;

public class ShoveAction : ActionBase {
    public ShoveAction(ObjectActions.ActionType actionType, string actionName, int index) : base(actionType, actionName, index) { }
    public override void Invoke(ActorBase actorBase, IObjectData objectData) {
        if (objectData.GetObject() is not RigidBody3D rigidBody3D) return;
        if (actorBase is not Player player) return;
        
        Vector3 tossDirection = -player.GetCamera().GlobalTransform.Basis.Z * 0.1f;
        rigidBody3D.LinearVelocity += tossDirection;
    }
}