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

    public static Vector2[] GetExtremes(params Vector2[] vectors) {
        if (vectors.Length == 0) return Array.Empty<Vector2>();

        float maxX = vectors.Max(v => v.X);
        float minX = vectors.Min(v => v.X);
        float maxY = vectors.Max(v => v.Y);
        float minY = vectors.Min(v => v.Y);

        Vector2 LXLY = new(maxX, maxY); // Largest X, Largest Y
        Vector2 LXSY = new(maxX, minY); // Largest X, Smallest Y
        Vector2 SXSY = new(minX, minY); // Smallest X, Smallest Y
        Vector2 SXLY = new(minX, maxY); // Smallest X, Largest Y

        return new[] { LXLY, LXSY, SXSY, SXLY };
    }

    public static Vector3[] GetExtremes(params Vector3[] vectors) {
        if (vectors.Length == 0) return Array.Empty<Vector3>();

        float maxX = vectors.Max(v => v.X);
        float minX = vectors.Min(v => v.X);
        float maxY = vectors.Max(v => v.Y);
        float minY = vectors.Min(v => v.Y);
        float maxZ = vectors.Max(v => v.Z);
        float minZ = vectors.Min(v => v.Z);

        Vector3 LXLYLZ = new(maxX, maxY, maxZ); // Largest X, Largest Y, Largest Z
        Vector3 LXLYSZ = new(maxX, maxY, minZ); // Largest X, Largest Y, Smallest Z
        Vector3 LXSYLZ = new(maxX, minY, maxZ); // Largest X, Smallest Y, Largest Z
        Vector3 LXSYSZ = new(maxX, minY, minZ); // Largest X, Smallest Y, Smallest Z
        Vector3 SXLYLZ = new(minX, maxY, maxZ); // Smallest X, Largest Y, Largest Z
        Vector3 SXLYSZ = new(minX, maxY, minZ); // Smallest X, Largest Y, Smallest Z
        Vector3 SXSYLZ = new(minX, minY, maxZ); // Smallest X, Smallest Y, Largest Z
        Vector3 SXSYSZ = new(minX, minY, minZ); // Smallest X, Smallest Y, Smallest Z

        return new[]{ LXLYLZ, LXLYSZ, LXSYLZ, LXSYSZ, SXLYLZ, SXLYSZ, SXSYLZ, SXSYSZ };
    }
}