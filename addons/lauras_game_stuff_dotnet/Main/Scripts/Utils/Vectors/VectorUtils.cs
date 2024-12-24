using Godot;

public static class VectorUtils {

    public static Vector3 RoundTo(Vector3 vector3, int significantDigits) {
        return new Vector3(
            Mathsf.Round(vector3.X, significantDigits),
            Mathsf.Round(vector3.Y, significantDigits),
            Mathsf.Round(vector3.Z, significantDigits)
            );
    }
    
}