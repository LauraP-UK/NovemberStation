using System;
using System.Collections.Generic;
using Godot;

public class ToastMessage : FormBase {
    private readonly TextureRectElement _icon;
    private readonly LabelElement _text;
    private readonly ColorRectElement _background;
    private readonly NinePatchRectElement _border;
    private readonly ControlElement _sizer;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ToastMessage.tscn",
        ICON = "Sizer/HBoxContainer/Control/Icon",
        TEXT = "Sizer/HBoxContainer/Text",
        BACKGROUND = "Sizer/BGColour",
        BORDER = "Sizer/Border",
        SIZER = "Sizer";
    
    private const int
        MESSAGE_BUFFER = 38,
        MAX_WIDTH = 600 - MESSAGE_BUFFER,
        MIN_FONT_SIZE = 10;

    public ToastMessage(string formName) : base(formName, FORM_PATH) {
        TextureRect icon = FindNode<TextureRect>(ICON);
        Label text = FindNode<Label>(TEXT);
        ColorRect background = FindNode<ColorRect>(BACKGROUND);
        NinePatchRect border = FindNode<NinePatchRect>(BORDER);
        Control sizer = FindNode<Control>(SIZER);

        text.SetLabelSettings((LabelSettings)text.GetLabelSettings().Duplicate());
        
        _icon = new TextureRectElement(icon);
        _text = new LabelElement(text);
        _background = new ColorRectElement(background);
        _border = new NinePatchRectElement(border);
        _sizer = new ControlElement(sizer);

        _menuElement = new ControlElement(_menu);
    }

    protected override List<IFormObject> GetAllElements() => new() { _icon, _text, _background, _border, _sizer };
    protected override void OnDestroy() { }

    public void SetIcon(string path) => _icon.SetTexture(path);

    public void SetText(string text) {
        Label label = _text.GetElement();
        label.SetText(text);

        LabelSettings labelSettings = label.LabelSettings;
        if (labelSettings == null) {
            GD.PrintErr("ERROR: LabelSettings not found on label!");
            return;
        }

        int fontsize = labelSettings.FontSize;

        Font font = label.GetThemeFont("");
        if (font == null) return;

        Vector2 textSize = font.GetStringSize(text, HorizontalAlignment.Left, -1, fontsize);

        while (textSize.X > MAX_WIDTH && fontsize > MIN_FONT_SIZE) {
            fontsize--;
            textSize = font.GetStringSize(text, HorizontalAlignment.Left, -1, fontsize);
        }

        labelSettings.FontSize = fontsize;
        label.LabelSettings = labelSettings;
        
        Vector2 minimumSize = _sizer.GetElement().GetCustomMinimumSize();
        Vector2 newSize = new(Math.Min(textSize.X, MAX_WIDTH) + MESSAGE_BUFFER, minimumSize.Y);
        _sizer.GetElement().SetCustomMinimumSize(newSize);
        _sizer.GetElement().SetSize(newSize);
    }

    public void SetBGColour(Color colour) => _background.SetColor(colour);

    public void SetAlpha(float alpha) {
        _icon.SetAlpha(alpha);
        _text.SetAlpha(alpha);
        _background.SetAlpha(alpha);
        _border.SetAlpha(alpha);
    }
}