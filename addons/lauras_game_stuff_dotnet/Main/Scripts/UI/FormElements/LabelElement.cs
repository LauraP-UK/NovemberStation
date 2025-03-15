using System;
using Godot;

public class LabelElement : FormElement<Label> {
    public LabelElement(Label element = null, Action<Label> onReady = null) : base(element, onReady) {}
    public LabelElement(string text, Action<Label> onReady = null) : base(text, onReady) {}
    
    public void SetText(string text) => GetElement().Text = text;
    
    public void SetAlpha(float alpha) => GetElement().Modulate = new Color(1, 1, 1, alpha);
    public void SetColor(Color color) {
        LabelSettings settings = GetElement().GetLabelSettings();
        settings.FontColor = color;
        GetElement().SetLabelSettings(settings);
    }
}