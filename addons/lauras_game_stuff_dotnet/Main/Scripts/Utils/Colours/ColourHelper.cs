using Godot;

public static class ColourHelper {

    public static Color GetFrom255(int r, int g, int b, float alpha) {
        float newR = r / 255f;
        float newG = g / 255f;
        float newB = b / 255f;
        return new Color(newR, newG, newB, alpha);
    }
}