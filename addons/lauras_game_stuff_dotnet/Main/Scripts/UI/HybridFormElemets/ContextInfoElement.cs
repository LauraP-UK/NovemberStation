
using System.Collections.Generic;
using Godot;

public class ContextInfoElement : FormBase {
    private readonly LabelElement _textLabel;
    private readonly ColorRectElement _bgColor;
    private string _contextText;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ContextInfoElement.tscn",
        BG_COLOUR = "BGColour",
        DISPLAY_LABEL_NAME = "ContextText";
    
    private const float BACKGROUND_ALPHA = 0.5f;

    public ContextInfoElement(string contextText) : base("contextText_txt", FORM_PATH) {
        _contextText = contextText;
        
        Label contextLabel = FindNode<Label>(DISPLAY_LABEL_NAME);
        ColorRect bgColor = FindNode<ColorRect>(BG_COLOUR);
        
        _textLabel = new LabelElement(contextLabel);
        _bgColor = new ColorRectElement(bgColor);
        
        _menuElement = new ControlElement(_menu);
        SetCaptureInput(false);
        
        _textLabel.GetElement().SetText(_contextText);
        GetNode().SetCustomMinimumSize(new Vector2(0, 20 * (_contextText.Count("\n") + 1)));
        _bgColor.SetAlpha(BACKGROUND_ALPHA);
    }
    protected override List<IFormObject> GetAllElements() => new() {_textLabel, _bgColor};
    protected override void OnDestroy() { }
    public override bool LockMovement() => false;
    
    public float GetMinimumWidth() => _textLabel.GetElement().GetMinimumSize().X;
    public float GetMinimumHeight() => _textLabel.GetElement().GetMinimumSize().Y;
    public string GetContext() => _contextText;
    
    public void HandleAlpha(float alpha) {
        _bgColor.SetAlpha(Mathsf.Lerp(0.0f, BACKGROUND_ALPHA, alpha));
        _textLabel.SetAlpha(alpha);
    }
}