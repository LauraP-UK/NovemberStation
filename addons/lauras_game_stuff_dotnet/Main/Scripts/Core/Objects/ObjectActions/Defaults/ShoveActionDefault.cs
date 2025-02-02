
using Godot;

public class ShoveActionDefault : BaseActionDefault {
    public static void Invoke(ActorBase actorBase, Node3D node, IEventBase ev, float force = 5.0f, MouseType mouseType = MouseType.NONE, KeyType keyType = KeyType.DOWN) {
        if (!CanRun(ev, mouseType) && !CanRun(ev, keyType)) return;
        if (node is not RigidBody3D rigidBody3D) return;
        if (actorBase is not Player player) return;
        Vector3 tossDirection = -player.GetCamera().GlobalTransform.Basis.Z * force;
        rigidBody3D.LinearVelocity += tossDirection;
    }
}