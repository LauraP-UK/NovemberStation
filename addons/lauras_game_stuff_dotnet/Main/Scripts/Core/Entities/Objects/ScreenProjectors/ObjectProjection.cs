
using System;
using Godot;

public class ObjectProjection<T> : IObjectProjection where T : Shape3D {
    private readonly Type _shapeType;
    private readonly Func<CollisionShape3D, Camera3D, Transform3D, (Vector2 min, Vector2 max)> _getScreenCorners;
    private ObjectProjection(Func<CollisionShape3D, Camera3D, Transform3D, (Vector2 min, Vector2 max)> getScreenCorners) {
        _shapeType = typeof(T);
        _getScreenCorners = getScreenCorners;
    }
    public (Vector2 min, Vector2 max) GetScreenCorners(CollisionShape3D shape, Camera3D camera, Transform3D transform) => _getScreenCorners(shape, camera, transform);
    public Type GetShapeType() => _shapeType;
    public static ObjectProjection<T> Create(Func<CollisionShape3D, Camera3D, Transform3D, (Vector2 min, Vector2 max)> getScreenCorners) => new(getScreenCorners);
}