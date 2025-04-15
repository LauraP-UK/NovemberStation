using System;
using Godot;

public class EaseType {
    private readonly string _name;
    private readonly Func<float, float, float, float> _ease;
    private EaseType(string name, Func<float, float, float, float> ease) {
        _name = name;
        _ease = ease;
    }

    public float Ease(float from, float to, float ratio) {
        ratio = Mathsf.Clamp(0.0f, 1.0f, ratio);
        return _ease(from, to, ratio);
    }

    public string GetName() => _name;

    public float Blend(float from, float to, float ratio, EaseType other, float otherStrength = 0.5f) {
        float thisVal = Ease(from, to, ratio);
        float otherVal = other.Ease(from, to, ratio);
        return Mathsf.Lerp(thisVal, otherVal, otherStrength);
    }

    /* --- CONSTRUCTORS --- */

    public static EaseType Create(string name, Func<float, float, float, float> ease) => new(name, ease);

    public static EaseType CreateBlend(EaseType a, EaseType b, float blendRatio) {
        float BlendFunc(float from, float to, float ratio) {
            float aVal = a.Ease(from, to, ratio);
            float bVal = b.Ease(from, to, ratio);
            return Mathsf.Lerp(aVal, bVal, blendRatio);
        }

        return Create(a.GetName() + "_" + b.GetName() + "_" + blendRatio + "_BLEND", BlendFunc);
    }

    public static EaseType Combine(EaseType a, EaseType b) {
        return Create(
            "COMBINE_" + a.GetName() + "_" + b.GetName(),
            (from, to, ratio) => {
                float mid = Mathf.Lerp(from, to, 0.5f);
                return ratio < 0.5f ? a.Ease(from, mid, ratio * 2) : b.Ease(mid, to, (ratio - 0.5f) * 2);
            }
        );
    }

    public static EaseType Quadratic(float power, bool easeIn) => Create(
        (easeIn ? "IN" : "OUT") + "_QUAD_" + power, (from, to, ratio) => Easing.CustomQuad(from, to, ratio, power, easeIn)
    );
}