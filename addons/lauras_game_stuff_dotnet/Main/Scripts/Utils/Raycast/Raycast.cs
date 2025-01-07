
using Godot;
using Godot.Collections;
using NovemberStation.Main;

public static class Raycast {
    
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
        PhysicsDirectSpaceState3D world = GetWorld();
        PhysicsRayQueryParameters3D parameters = new() {
            From = start,
            To = end,
            CollisionMask = uint.MaxValue
        };

        RaycastResult raycastResult = new(start, end);
        Array<Rid> excludedObjects = new();
        Vector3 currentStart = start;

        while (true) {
            parameters.From = currentStart;
            parameters.Exclude = excludedObjects;

            Dictionary result = world.IntersectRay(parameters);

            if (result.Count == 0) break; // No more hits

            GodotObject hitObject = (GodotObject) result["collider"];
            Vector3 hitPosition = (Vector3)result["position"];
            Vector3 hitNormal = (Vector3)result["normal"];
            Rid hitRid = (Rid)result["rid"];
            float distance = start.DistanceTo(hitPosition);

            raycastResult.AddHitBody(distance, hitObject as Node3D, hitPosition, hitNormal);
            excludedObjects.Add(hitRid);

            currentStart = hitPosition + (end - start).Normalized() * 0.01f;

            if (hitPosition.DistanceTo(end) < 0.01f) break; // Feasibly at the end
        }
        return raycastResult;
    }
    
    private static PhysicsDirectSpaceState3D GetWorld() => PhysicsServer3D.SpaceGetDirectState(TestScript.I().GetWorldRid());
}