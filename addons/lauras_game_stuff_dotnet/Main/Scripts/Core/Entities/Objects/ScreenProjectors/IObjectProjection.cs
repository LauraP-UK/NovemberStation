using System;
using Godot;

public interface IObjectProjection {
    public (Vector2 min, Vector2 max) GetScreenCorners(CollisionShape3D shape, Camera3D camera, Transform3D transform);
    public Type GetShapeType();
}