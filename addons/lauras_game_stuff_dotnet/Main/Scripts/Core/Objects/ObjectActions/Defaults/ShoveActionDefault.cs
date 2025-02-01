
using Godot;

public class ShoveActionDefault : BaseActionDefault {
    public static void Invoke(ActorBase actorBase, Node3D node, IEventBase ev) {
        if (!CanRun(ev, MouseType.DOWN)) return;
        if (node is not RigidBody3D rigidBody3D) return;
        if (actorBase is not Player player) return;
        Vector3 tossDirection = -player.GetCamera().GlobalTransform.Basis.Z * 5.0f;
        rigidBody3D.LinearVelocity += tossDirection;
    }
}