using System;
using System.Linq;
using Godot;

public static class VectorUtils {

    public static Vector3 RoundTo(Vector3 vector3, int significantDigits) {
        return new Vector3(
            Mathsf.Round(vector3.X, significantDigits),
            Mathsf.Round(vector3.Y, significantDigits),
            Mathsf.Round(vector3.Z, significantDigits)
            );
    }

    public static ExtremesInfo2D GetExtremes(params Vector2[] vectors) {
        if (vectors.Length == 0) return ExtremesInfo2D.Empty();

        float maxX = vectors.Max(v => v.X);
        float minX = vectors.Min(v => v.X);
        float maxY = vectors.Max(v => v.Y);
        float minY = vectors.Min(v => v.Y);

        return new ExtremesInfo2D(new[] {
            new Vector2(maxX, maxY), 
            new Vector2(maxX, minY), 
            new Vector2(minX, minY), 
            new Vector2(minX, maxY)
        });
    }
    
    public class ExtremesInfo2D {
        public readonly Vector2 
            LXLY,
            LXSY,
            SXSY,
            SXLY,
            min, max, centre;
        public ExtremesInfo2D(Vector2[] corners) {
            LXLY = corners[0];
            LXSY = corners[1];
            SXSY = corners[2];
            SXLY = corners[3];
            min = SXSY;
            max = LXLY;
            centre = (min + max) / 2;
        }
        public static ExtremesInfo2D Empty() => new(new[]{Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero});
    }

    public static ExtremesInfo3D GetExtremes(params Vector3[] vectors) {
        if (vectors.Length == 0) return ExtremesInfo3D.Empty();

        float maxX = vectors.Max(v => v.X);
        float minX = vectors.Min(v => v.X);
        float maxY = vectors.Max(v => v.Y);
        float minY = vectors.Min(v => v.Y);
        float maxZ = vectors.Max(v => v.Z);
        float minZ = vectors.Min(v => v.Z);

        return new ExtremesInfo3D(new[] {
            new Vector3(maxX, maxY, maxZ),
            new Vector3(maxX, maxY, minZ),
            new Vector3(maxX, minY, maxZ),
            new Vector3(maxX, minY, minZ),
            new Vector3(minX, maxY, maxZ),
            new Vector3(minX, maxY, minZ),
            new Vector3(minX, minY, maxZ),
            new Vector3(minX, minY, minZ)
        });
    }
    
    public class ExtremesInfo3D {
        public readonly Vector3 
            LXLYLZ,
            LXLYSZ,
            LXSYLZ,
            LXSYSZ,
            SXLYLZ,
            SXLYSZ,
            SXSYLZ,
            SXSYSZ,
            min, max, centre;
        public ExtremesInfo3D(Vector3[] corners) {
            LXLYLZ = corners[0];
            LXLYSZ = corners[1];
            LXSYLZ = corners[2];
            LXSYSZ = corners[3];
            SXLYLZ = corners[4];
            SXLYSZ = corners[5];
            SXSYLZ = corners[6];
            SXSYSZ = corners[7];
            min = SXSYSZ;
            max = LXLYLZ;
            centre = (min + max) / 2;
        }
        public static ExtremesInfo3D Empty() => new(new[]{Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero});
    }
}