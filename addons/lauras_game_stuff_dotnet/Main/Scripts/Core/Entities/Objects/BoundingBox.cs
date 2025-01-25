
using Godot;

public class BoundingBox {
    
    private readonly Vector3 _PXPYPZ, _PXPYNZ, _PXNYPZ, _PXNYNZ, _NXPYPZ, _NXPYNZ, _NXNYPZ, _NXNYNZ;
    
    public BoundingBox(Vector3 size) {
        _PXPYPZ = new Vector3(size.X, size.Y, size.Z);
        _PXPYNZ = new Vector3(size.X, size.Y, -size.Z);
        _PXNYPZ = new Vector3(size.X, -size.Y, size.Z);
        _PXNYNZ = new Vector3(size.X, -size.Y, -size.Z);
        _NXPYPZ = new Vector3(-size.X, size.Y, size.Z);
        _NXPYNZ = new Vector3(-size.X, size.Y, -size.Z);
        _NXNYPZ = new Vector3(-size.X, -size.Y, size.Z);
        _NXNYNZ = new Vector3(-size.X, -size.Y, -size.Z);
    }

    public static BoundingBox FromCollisionMesh(CollisionShape3D shape) {
        return new BoundingBox(Vector3.Zero);
    }
    
}