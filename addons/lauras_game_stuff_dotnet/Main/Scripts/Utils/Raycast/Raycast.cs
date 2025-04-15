using Godot;
using Godot.Collections;

public static class Raycast {
    public static RaycastResult TraceActive(float distance) {
        Camera3D camera3D = GameManager.I().GetActiveCamera();
        Vector3 origin = camera3D.GetGlobalTransform().Origin;
        Vector3 forward = -camera3D.GetGlobalTransform().Basis.Z.Normalized();
        Vector3 target = origin + forward * distance;

        return Trace(origin, target);
    }

    public static RaycastResult TraceActive(Node3D to) {
        Camera3D camera3D = GameManager.I().GetActiveCamera();
        Vector3 origin = camera3D.GetGlobalTransform().Origin;
        return Trace(origin, to.GlobalPosition);
    }

    public static RaycastResult TraceActive(Vector3 end) {
        Camera3D camera3D = GameManager.I().GetActiveCamera();
        Vector3 origin = camera3D.GetGlobalTransform().Origin;
        return Trace(origin, end);
    }

    public static RaycastResult Trace(IViewable actor, Vector3 end) {
        Vector3 origin = actor.GetCamera().GlobalTransform.Origin;
        return Trace(origin, end);
    }

    public static RaycastResult Trace(IViewable actor, float distance) {
        Vector3 origin = actor.GetCamera().GlobalTransform.Origin;
        Vector3 forward = -actor.GetCamera().GlobalTransform.Basis.Z.Normalized();
        Vector3 target = origin + forward * distance;

        return Trace(origin, target);
    }

    public static RaycastResult Trace(Vector3 start, float distance, Vector3 direction) {
        Vector3 end = start + direction.Normalized() * distance;
        return Trace(start, end);
    }

    public static RaycastResult Trace(Vector3 start, Vector3 end) {
        return Trace(start, end, null);
    }

    public static RaycastResult Trace(Vector3 start, Vector3 end, params CollisionObject3D[] ignore) {
        PhysicsDirectSpaceState3D world = GetWorld();
        PhysicsRayQueryParameters3D parameters = new() {
            From = start,
            To = end,
            CollisionMask = uint.MaxValue,
            CollideWithAreas = true
        };

        RaycastResult raycastResult = new(start, end);
        SmartSet<Node> ignoreNodes = [];
        if (ignore != null) foreach (CollisionObject3D obj in ignore) ignoreNodes.Add(GameUtils.FindSceneRoot(obj));
        Array<Rid> excludedObjects = [];

        Vector3 currentStart = start;

        while (true) {
            parameters.From = currentStart;
            parameters.Exclude = excludedObjects;

            Dictionary result = world.IntersectRay(parameters);

            if (result.Count == 0) break; // No more hits

            GodotObject hitObject = (GodotObject)result["collider"];
            Vector3 hitPosition = (Vector3)result["position"];
            Vector3 hitNormal = (Vector3)result["normal"];
            Rid hitRid = (Rid)result["rid"];
            float distance = start.DistanceTo(hitPosition);
            
            Node sceneRoot = GameUtils.FindSceneRoot(hitObject as Node);

            Node hitNode = sceneRoot is not Node3D ? hitObject as Node : sceneRoot;
            if (hitNode == null) GD.PrintErr($"ERROR: Raycast.Trace() : Failed to find root Node3D for hit object '{hitObject}'.");
            if (hitNode is not Node3D) GD.PrintErr($"ERROR: Raycast.Trace() : Hit object '{hitNode}' is not a Node3D. Got '{(hitNode == null ? "NULL" : hitNode.GetType())}'.");

            excludedObjects.Add(hitRid);
            if (!ignoreNodes.Add(hitNode)) continue;

            raycastResult.AddHitBody(distance, hitNode as Node3D, hitPosition, hitNormal);
            currentStart = hitPosition + (end - start).Normalized() * 0.01f;

            if (hitPosition.DistanceTo(end) < 0.01f) break; // Feasibly at the end
        }

        return raycastResult;
    }

    private static PhysicsDirectSpaceState3D GetWorld() => PhysicsServer3D.SpaceGetDirectState(GameManager.I().GetWorldRid());
}