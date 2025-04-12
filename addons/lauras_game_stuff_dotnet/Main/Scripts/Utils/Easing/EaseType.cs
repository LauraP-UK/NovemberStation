using System;
using Godot;

public class EaseType {
    private readonly Func<float, float, float, float> _ease;
    private EaseType(Func<float, float, float, float> ease) => _ease = ease;
    public float Ease(float from, float to, float ratio) {
        ratio = Mathsf.Clamp(0.0f, 1.0f, ratio);
        return _ease(from, to, ratio);
    }
    public float Blend(float from, float to, float ratio, EaseType other, float otherStrength = 0.5f) {
        float thisVal = Ease(from, to, ratio);
        float otherVal = other.Ease(from, to, ratio);
        return Mathsf.Lerp(thisVal, otherVal, otherStrength);
    }
    public static EaseType Create(Func<float, float, float, float> ease) => new(ease);
    public static EaseType Combine(EaseType a, EaseType b) {
        return Create(
            (from, to, ratio) => {
                float mid = Mathf.Lerp(from, to, 0.5f);
                return ratio < 0.5f ? a.Ease(from, mid, ratio * 2) : b.Ease(mid, to, (ratio - 0.5f) * 2);
            }
        );
    }
    public static EaseType Quadratic(float power, bool easeIn) => Create((from, to, ratio) => Easing.CustomQuad(from, to, ratio, power, easeIn));
}