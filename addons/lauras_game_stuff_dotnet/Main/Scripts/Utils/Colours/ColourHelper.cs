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
}