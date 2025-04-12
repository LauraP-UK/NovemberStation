using System;
using Godot;

public class Easing {
    public static readonly EaseType LINEAR = EaseType.Create(Mathf.Lerp);
    public static readonly EaseType IN_SINE = EaseType.Create((from, to, ratio) => Mathf.Lerp(from, to, Mathf.Sin(ratio * Mathf.Pi / 2)));
    public static readonly EaseType OUT_SINE = EaseType.Create((from, to, ratio) => Mathf.Lerp(from, to, 1 - Mathf.Cos(ratio * Mathf.Pi / 2)));
    public static readonly EaseType OUT_IN_SINE = EaseType.Combine(OUT_SINE, IN_SINE);
    public static readonly EaseType IN = EaseType.Quadratic(2.0f, true);
    public static readonly EaseType OUT = EaseType.Quadratic(2.0f, false);
    public static readonly EaseType OUT_IN = EaseType.Combine(OUT, IN);
    public static readonly EaseType IN_CIRC = EaseType.Create((from, to, ratio) => (float)Mathf.Lerp(from, to, Math.Sqrt(1.0f - Math.Pow(ratio - 1.0f, 2))));
    public static readonly EaseType OUT_CIRC = EaseType.Create((from, to, ratio) => (float)Mathf.Lerp(from, to, 1.0f - Math.Sqrt(1.0f - ratio * ratio)));
    public static readonly EaseType OUT_IN_CIRC = EaseType.Combine(OUT_CIRC, IN_CIRC);

    public static float CustomQuad(float from, float to, float ratio, float power, bool easeIn = true) =>
        (float)Mathf.Lerp(from, to, easeIn ? 1.0f - Math.Pow(1.0f - ratio, power) : Math.Pow(ratio, power));
}