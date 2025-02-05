
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class BoundingBox {
    
    private readonly Vector3[] _corners;
    private readonly Vector3 _min, _max, _centre;

    private static readonly int[,] EDGE_MATRIX;

    static BoundingBox() {
        EDGE_MATRIX = new[,] {
            { 0, 1 }, { 0, 2 }, { 0, 4 }, // Edges connected to corner 0
            { 1, 3 }, { 1, 5 },           // Edges connected to corner 1
            { 2, 3 }, { 2, 6 },           // Edges connected to corner 2
            { 3, 7 },                     // Edges connected to corner 3
            { 4, 5 }, { 4, 6 },           // Edges connected to corner 4
            { 5, 7 },                     // Edges connected to corner 5
            { 6, 7 }                      // Edges connected to corner 6
        };
    }
    
    public BoundingBox(Vector3 size) {
        _corners = new Vector3[8];
        _corners[0] = new Vector3(size.X, size.Y, size.Z);
        _corners[1] = new Vector3(size.X, size.Y, -size.Z);
        _corners[2] = new Vector3(size.X, -size.Y, size.Z);
        _corners[3] = new Vector3(size.X, -size.Y, -size.Z);
        _corners[4] = new Vector3(-size.X, size.Y, size.Z);
        _corners[5] = new Vector3(-size.X, size.Y, -size.Z);
        _corners[6] = new Vector3(-size.X, -size.Y, size.Z);
        _corners[7] = new Vector3(-size.X, -size.Y, -size.Z);
        _min = new Vector3(
            _corners.Min(corner => corner.X),
            _corners.Min(corner => corner.Y),
            _corners.Min(corner => corner.Z)
        );
        _max = new Vector3(
            _corners.Max(corner => corner.X),
            _corners.Max(corner => corner.Y),
            _corners.Max(corner => corner.Z)
        );
        _centre = (_min + _max) / 2;
    }
    
    public BoundingBox(Vector3[] corners) {
        _corners = corners;
        _min = new Vector3(
            _corners.Min(corner => corner.X),
            _corners.Min(corner => corner.Y),
            _corners.Min(corner => corner.Z)
        );
        _max = new Vector3(
            _corners.Max(corner => corner.X),
            _corners.Max(corner => corner.Y),
            _corners.Max(corner => corner.Z)
        );
        _centre = (_min + _max) / 2;
    }

    public Vector3[] GetCorners() => _corners;
    
    public List<KeyValuePair<Vector3, Vector3>> GetEdges() {
        List<KeyValuePair<Vector3, Vector3>> edges = new();
        for (int i = 0; i < EDGE_MATRIX.GetLength(0); i++) {
            int startIdx = EDGE_MATRIX[i, 0];
            int endIdx = EDGE_MATRIX[i, 1];
            edges.Add(new KeyValuePair<Vector3, Vector3>(_corners[startIdx], _corners[endIdx]));
        }
        return edges;
    }
    
    public Vector3 GetCenter() {
        Vector3 center = _corners.Aggregate(Vector3.Zero, (current, corner) => current + corner);
        return center / _corners.Length;
    }

    public Vector3[] GetCornersInWorldSpace(Transform3D transform) {
        Vector3[] worldCorners = new Vector3[_corners.Length];
        for (int i = 0; i < _corners.Length; i++) worldCorners[i] = transform * _corners[i];
        return worldCorners;
    }
    
    public List<KeyValuePair<Vector3, Vector3>> GetEdgesInWorldSpace(Transform3D transform) {
        Vector3[] worldCorners = GetCornersInWorldSpace(transform);
        List<KeyValuePair<Vector3, Vector3>> edges = new();
        for (int i = 0; i < EDGE_MATRIX.GetLength(0); i++) {
            int startIdx = EDGE_MATRIX[i, 0];
            int endIdx = EDGE_MATRIX[i, 1];
            edges.Add(new KeyValuePair<Vector3, Vector3>(worldCorners[startIdx], worldCorners[endIdx]));
        }
        return edges;
    }
    
    public Vector2[] GetCornersInScreenSpace(Camera3D camera, Transform3D transform3D) {
        Vector3[] worldCorners = GetCornersInWorldSpace(transform3D);
        Vector2[] screenCorners = new Vector2[worldCorners.Length];
        for (int i = 0; i < worldCorners.Length; i++)
            screenCorners[i] = camera.UnprojectPosition(worldCorners[i]);
        return screenCorners;
    }
    
    public void DrawDebugLines(Transform3D transform3D, Color color) {
        List<KeyValuePair<Vector3, Vector3>> edges = GetEdgesInWorldSpace(transform3D);
        foreach (KeyValuePair<Vector3, Vector3> edge in edges) DebugDraw.Line(edge.Key, edge.Value, color);
    }

    public static BoundingBox FromCollisionMesh(CollisionShape3D shape) {
        switch (shape.Shape) {
            case BoxShape3D boxShape:
                return new BoundingBox(boxShape.Size / 2);
            case SphereShape3D sphereShape:
                return new BoundingBox(sphereShape.Radius * Vector3.One);
            case CapsuleShape3D capsuleShape: {
                float radius = capsuleShape.Radius;
                float height = capsuleShape.Height / 2;
                return new BoundingBox(new Vector3(radius, height + radius, radius));
            }
            default:
                throw new NotSupportedException($"ERROR: BoundingBox.FromCollisionMesh() : Collision shape type not supported: {shape.Shape.GetType()}");
        }
    }
}