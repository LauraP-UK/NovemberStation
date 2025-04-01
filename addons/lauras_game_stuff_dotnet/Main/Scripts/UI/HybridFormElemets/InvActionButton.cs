using System.Collections.Generic;
using Godot;

public class InvActionButton : FormBase {
    
    private readonly LabelElement _actionLabel, _leftArrow, _rightArrow;
    private readonly ButtonElement _button;
    private readonly ColorRectElement _bgColour;
    
    private bool _isDisabled = false;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/InvActionButton.tscn",
        ACTION_LABEL = "Content/ActionName",
        LEFT_ARROW = "Content/LeftArrow",
        RIGHT_ARROW = "Content/RightArrow",
        BUTTON = "SelectButton",
        BG_COLOUR = "BGContainer/BGColour";

    private static readonly Color
        DEFAULT_BG_COLOR = new(0.05f, 0.05f, 0.05f),
        FOCUS_BG_COLOR = Colors.DimGray,
        SELECTED_BG_COLOR = Colors.Gray;

    public InvActionButton() : base("inv_action_btn", FORM_PATH) {
        Label actionLabel = FindNode<Label>(ACTION_LABEL);
        Label leftArrow = FindNode<Label>(LEFT_ARROW);
        Label rightArrow = FindNode<Label>(RIGHT_ARROW);
        Button button = FindNode<Button>(BUTTON);
        ColorRect bgColour = FindNode<ColorRect>(BG_COLOUR);
        
        _actionLabel = new LabelElement(actionLabel);
        _leftArrow = new LabelElement(leftArrow);
        _rightArrow = new LabelElement(rightArrow);
        _button = new ButtonElement(button);
        _bgColour = new ColorRectElement(bgColour);
        
        _button.AddAction(Control.SignalName.MouseEntered, _ => {
            if (_isDisabled) return;
            _bgColour.SetColor(FOCUS_BG_COLOR);
        });
        _button.AddAction(Control.SignalName.MouseExited, _ => {
            if (_isDisabled) return;
            _bgColour.SetColor(DEFAULT_BG_COLOR);
        });
        _button.OnButtonDown(_ => {
            if (_isDisabled) return;
            VisualPress(true);
        });
        _button.OnButtonUp(_ => {
            if (_isDisabled) return;
            VisualPress(false);
        });
        
        _menuElement = new ControlElement(_menu);
        Disable();
        
        _menu.SetCustomMinimumSize(new Vector2(0, 50));
    }
    protected override List<IFormObject> GetAllElements() => new() { _actionLabel, _leftArrow, _rightArrow, _button, _bgColour };
    protected override void OnDestroy() { }
    
    public ButtonElement GetButton() => _button;
    public LabelElement GetActionLabel() => _actionLabel;
    
    public void SetActionName(string name) => _actionLabel.SetText(name);
    
    public void ShowLeftArrow(bool show) => _leftArrow.SetAlpha(show ? 1.0f : 0.0f);
    public void ShowRightArrow(bool show) => _rightArrow.SetAlpha(show ? 1.0f : 0.0f);

    public void SetStore() {
        SetActionName("Store");
        GetActionLabel().SetAlpha(1.0f);
        ShowLeftArrow(false);
        ShowRightArrow(true);
        _bgColour.SetColor(DEFAULT_BG_COLOR);
        _isDisabled = false;
    }
    public void SetTake() {
        SetActionName("Take");
        GetActionLabel().SetAlpha(1.0f);
        ShowLeftArrow(true);
        ShowRightArrow(false);
        _bgColour.SetColor(DEFAULT_BG_COLOR);
        _isDisabled = false;
    }
    public void SetMisc(string text) {
        SetActionName(text);
        GetActionLabel().SetAlpha(1.0f);
        ShowLeftArrow(false);
        ShowRightArrow(false);
        _bgColour.SetColor(DEFAULT_BG_COLOR);
        _isDisabled = false;
    }
    public void Disable(string text = "Select an item") {
        SetActionName(text);
        GetActionLabel().SetAlpha(0.5f);
        ShowLeftArrow(false);
        ShowRightArrow(false);
        _bgColour.SetColor(DEFAULT_BG_COLOR);
        _isDisabled = true;
    }
    public void VisualPress(bool pressed) => _bgColour.SetColor(pressed ? SELECTED_BG_COLOR : FOCUS_BG_COLOR);
}