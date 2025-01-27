using System;
using Godot;

public class ColorRectElement : FormElement<ColorRect> {
    
    public ColorRectElement(ColorRect element = null, Action<ColorRect> onReady = null) : base(element, onReady) { }
    public ColorRectElement(string text, Action<ColorRect> onReady = null) : base(text, onReady) { }
    
    public void SetSize(Vector2 size) => GetElement().SetCustomMinimumSize(size);
    
    public void SetColor(Color color) => GetElement().Color = color;
    public void SetColor(float r, float g, float b, float a) => SetColor(new Color(r, g, b, a));
    public void SetAlpha(float alpha) => GetElement().SelfModulate = new Color(1, 1, 1, alpha);
    public float GetAlpha() => GetElement().SelfModulate.A;
}