
using System;
using System.Linq;
using System.Reflection;
using Godot;

public static class Projections {
    public static readonly IObjectProjection BOX = ObjectProjection<BoxShape3D>.Create(
        (shape, camera, transform) => {
            BoundingBox bb = BoundingBox.FromCollisionMesh(shape);

            Vector2[] inScreenSpace = bb.GetCornersInScreenSpace(camera, transform);
            VectorUtils.ExtremesInfo2D vecExtremes = VectorUtils.GetExtremes(inScreenSpace);

            Vector2 minPos = vecExtremes.min;
            Vector2 maxPos = vecExtremes.max - minPos;
            return (minPos, maxPos);
        });
    public static readonly IObjectProjection SPHERE = ObjectProjection<SphereShape3D>.Create(
        (shape, camera, transform) => {
            if (shape.Shape is not SphereShape3D sphere) return (default, default);

            Vector3 center = transform.Origin;
            Vector3 right = camera.GlobalTransform.Basis.X.Normalized();
            Vector3 surfacePoint = center + right * sphere.Radius;
            Vector2 screenCenter = camera.UnprojectPosition(center);
            Vector2 screenEdge = camera.UnprojectPosition(surfacePoint);
            float screenRadius = screenEdge.DistanceTo(screenCenter);

            Vector2 min = screenCenter - new Vector2(screenRadius, screenRadius);
            Vector2 max = screenCenter + new Vector2(screenRadius, screenRadius);

            return (min, max - min);
        });
    public static readonly IObjectProjection CYLINDER = ObjectProjection<CylinderShape3D>.Create(
        (shape, camera, transform) => {
            if (shape.Shape is not CylinderShape3D cylinder) return (default, default);

            float height = cylinder.Height / 2f;
            float radius = cylinder.Radius;

            const int SAMPLE_COUNT = 16;
            Vector3[] points = new Vector3[SAMPLE_COUNT * 2];

            for (int i = 0; i < SAMPLE_COUNT; i++) {
                float angle = Mathf.Tau * i / SAMPLE_COUNT;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                points[i] = transform * new Vector3(x, +height, z);
                points[i + SAMPLE_COUNT] = transform * new Vector3(x, -height, z);
            }

            Vector2[] screenPoints = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
                screenPoints[i] = camera.UnprojectPosition(points[i]);

            VectorUtils.ExtremesInfo2D extremes = VectorUtils.GetExtremes(screenPoints);
            Vector2 minPos = extremes.min;
            Vector2 maxPos = extremes.max - minPos;
            return (minPos, maxPos);
        });
    public static readonly IObjectProjection CAPSULE = ObjectProjection<CapsuleShape3D>.Create(
        (shape, camera, transform) => {
            if (shape.Shape is not CapsuleShape3D capsule) return (default, default);

            float height = capsule.Height / 2f;
            float radius = capsule.Radius;

            Vector3[] localPoints = {
                new(0, height + radius, 0),
                new(0, -height - radius, 0),
                
                new(+radius, 0, 0),
                new(-radius, 0, 0),
                new(0, 0, +radius),
                new(0, 0, -radius),
                
                new(+radius, +height, 0),
                new(0, +height, +radius),
                
                new(-radius, -height, 0),
                new(0, -height, -radius)
            };

            Vector3[] worldPoints = new Vector3[localPoints.Length];
            for (int i = 0; i < localPoints.Length; i++)
                worldPoints[i] = transform * localPoints[i];

            Vector2[] screenPoints = new Vector2[worldPoints.Length];
            for (int i = 0; i < worldPoints.Length; i++)
                screenPoints[i] = camera.UnprojectPosition(worldPoints[i]);

            VectorUtils.ExtremesInfo2D extremes = VectorUtils.GetExtremes(screenPoints);
            Vector2 minPos = extremes.min;
            Vector2 maxPos = extremes.max - minPos;
            return (minPos, maxPos);
        });

    private static readonly IObjectProjection[] _all;

    static Projections() {
        _all = typeof(Projections)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => typeof(IObjectProjection).IsAssignableFrom(field.FieldType))
            .Select(field => field.GetValue(null) as IObjectProjection)
            .Where(p => p != null)
            .ToArray();
        GD.Print("[Projections] INFO: Registered all object projections.");
    }

    public static (Vector2 min, Vector2 max) Project(CollisionShape3D shape, Camera3D camera, Transform3D transform) {
        Type matchType = shape.Shape.GetType();
        foreach (IObjectProjection projection in _all)
            if (projection.GetShapeType() == matchType)
                return projection.GetScreenCorners(shape, camera, transform);
        GD.PrintErr($"ERROR: Projections.Project() : No projection found for shape type: {matchType.Name}");
        return (default, default);
    }
}