
using System.Collections.Generic;
using Godot;

public class NameDisplay : FormBase {
    private readonly LabelElement _nameLabel;
    private readonly ColorRectElement _bgColor;
    private readonly NinePatchRectElement _frame;
    private readonly string _displayName;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/NameDisplayElement.tscn",
        BG_COLOUR = "BGColour",
        DISPLAY_LABEL_NAME = "DisplayName",
        DISPLAY_FRAME = "Frame";
    
    private const float BACKGROUND_ALPHA = 0.5f;

    public NameDisplay(string displayName, float minimumHeight) : base(displayName + "_txt", FORM_PATH) {
        _displayName = displayName;
        
        Label nameLabel = FindNode<Label>(DISPLAY_LABEL_NAME);
        ColorRect bgColor = FindNode<ColorRect>(BG_COLOUR);
        NinePatchRect frame = FindNode<NinePatchRect>(DISPLAY_FRAME);
        
        _nameLabel = new LabelElement(nameLabel);
        _bgColor = new ColorRectElement(bgColor);
        _frame = new NinePatchRectElement(frame);
        
        _menuElement = new ControlElement(_menu);
        SetCaptureInput(false);
        
        _nameLabel.SetText(_displayName);
        GetNode().SetCustomMinimumSize(new Vector2(0, minimumHeight));
        _bgColor.SetAlpha(BACKGROUND_ALPHA);
    }
    protected override List<IFormObject> GetAllElements() => new() {_nameLabel, _bgColor, _frame};
    protected override void OnDestroy() { }
    public override bool LockMovement() => false;
    
    public float GetMinimumWidth() => _nameLabel.GetElement().GetMinimumSize().X;
    public string GetDisplayName() => _displayName;
    
    public void HandleAlpha(float alpha) {
        _bgColor.SetAlpha(Mathsf.Lerp(0.0f, BACKGROUND_ALPHA, alpha));
        _nameLabel.SetAlpha(alpha);
        _frame.SetAlpha(alpha);
    }
}