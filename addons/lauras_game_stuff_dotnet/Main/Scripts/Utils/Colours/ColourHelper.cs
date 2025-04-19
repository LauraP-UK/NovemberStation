using System;
using Godot;

public static class ColourHelper {

    public static Color GetFrom255(int r, int g, int b, float alpha) {
        float newR = r / 255f;
        float newG = g / 255f;
        float newB = b / 255f;
        return new Color(newR, newG, newB, alpha);
    }

    public static Color Ease(Color a, Color b, float ratio, EaseType ease) {
        return new Color(
            ease.Ease(a.R, b.R, ratio),
            ease.Ease(a.G, b.G, ratio),
            ease.Ease(a.B, b.B, ratio),
            ease.Ease(a.A, b.A, ratio)
            );
    }

    public static Color CycleRainbow(float ratio) {
        ratio = Math.Clamp(ratio, 0.0f, 1.0f);
        float stage = ratio * 6.0f;
        float r = 0.0f, g = 0.0f, b = 0.0f;
        switch (stage) {
            case >= 0.0f and < 1.0f:
                r = 1.0f;
                g = Mathsf.Remap(0.0f, 1.0f, stage, 0.0f, 1.0f);
                break;
            case >= 1.0f and < 2.0f:
                r = Mathsf.Remap(1.0f, 2.0f, stage, 1.0f, 0.0f);
                g = 1.0f;
                break;
            case >= 2.0f and < 3.0f:
                g = 1.0f;
                b = Mathsf.Remap(2.0f, 3.0f, stage, 0.0f, 1.0f);
                break;
            case >= 3.0f and < 4.0f:
                g = Mathsf.Remap(3.0f, 4.0f, stage, 1.0f, 0.0f);
                b = 1.0f;
                break;
            case >= 4.0f and < 5.0f:
                b = 1.0f;
                r = Mathsf.Remap(4.0f, 5.0f, stage, 0.0f, 1.0f);
                break;
            default:
                b = Mathsf.Remap(5.0f, 6.0f, stage, 1.0f, 0.0f);
                r = 1.0f;
                break;
        }
        return new Color(r, g, b);
    }
}