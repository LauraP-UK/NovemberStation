using System.Collections.Generic;
using System.Linq;
using Godot;

public class RaycastResult {
    private readonly Vector3 _start, _end;
    private readonly List<HitBodyData> _hitBodies = new();

    public RaycastResult(Vector3 start, Vector3 end) {
        _start = start;
        _end = end;
    }

    public Vector3 GetStart() => _start;
    public Vector3 GetEnd() => _end;

    public bool HasHit() => HitCount() > 0;
    public int HitCount() => _hitBodies.Count;

    public List<HitBodyData> GetHitsSortedByDistance() => _hitBodies.OrderBy(hit => hit.Distance).ToList();
    public HitBodyData GetClosestHit() => _hitBodies.OrderBy(hit => hit.Distance).FirstOrDefault();
    public HitBodyData GetViaBody(Node3D body) => _hitBodies.FirstOrDefault(hit => hit.Body.Equals(body));
    public HitBodyData GetViaRoot(Node3D root) => _hitBodies.FirstOrDefault(hit => hit.Root.Equals(root));

    public void AddHitBody(float distance, Node3D body, Node3D root, Vector3 hitAtPosition, Vector3 hitNormal) =>
        _hitBodies.Add(new HitBodyData {
            Body = body,
            Root = root,
            HitAtPosition = hitAtPosition,
            HitNormal = hitNormal,
            Distance = distance
        });

    public class HitBodyData {
        public Node3D Body { get; init; }
        public Node3D Root { get; init; }
        public Vector3 HitAtPosition { get; init; }
        public Vector3 HitNormal { get; init; }
        public float Distance { get; init; }

        public float DistanceTo(Vector3 point) => HitAtPosition.DistanceTo(point);
    }
}