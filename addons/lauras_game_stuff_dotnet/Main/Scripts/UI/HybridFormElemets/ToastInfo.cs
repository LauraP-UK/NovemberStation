
using System.Collections.Generic;
using Godot;

public class ToastInfo : FormBase {
    private readonly TextureRectElement _icon;
    private readonly LabelElement _text;
    private readonly ColorRectElement _background;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ToastInfo.tscn",
        ICON = "HBoxContainer/Control/Icon",
        TEXT = "HBoxContainer/Text",
        BACKGROUND = "BGColour";
    
    public ToastInfo(string formName) : base(formName, FORM_PATH) {
        TextureRect icon = FindNode<TextureRect>(ICON);
        Label text = FindNode<Label>(TEXT);
        ColorRect background = FindNode<ColorRect>(BACKGROUND);
        
        _icon = new TextureRectElement(icon);
        _text = new LabelElement(text);
        _background = new ColorRectElement(background);
        
        _menuElement = new ControlElement(_menu);
    }
    protected override List<IFormObject> GetAllElements() => new() { _icon, _text, _background };
    protected override void OnDestroy() { }
    
    public void SetIcon(string path) => _icon.SetTexture(path);
    public void SetText(string text) => _text.GetElement().SetText(text);
    public void SetBGColour(Color colour) => _background.SetColor(colour);
}