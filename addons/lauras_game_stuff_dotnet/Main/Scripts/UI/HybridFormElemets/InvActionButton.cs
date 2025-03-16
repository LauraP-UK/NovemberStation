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
        DEFAULT_BG_COLOR = new(0.1f, 0.1f, 0.1f),
        FOCUS_BG_COLOR = Colors.DimGray,
        DISABLED_BG_COLOUR = new(0.05f, 0.05f, 0.05f);

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
        
        _button.AddAction(Control.SignalName.FocusEntered, _ => {
            if (_isDisabled) return;
            _bgColour.SetColor(FOCUS_BG_COLOR);
        });
        _button.AddAction(Control.SignalName.FocusExited, _ => {
            if (_isDisabled) return;
            _bgColour.SetColor(DEFAULT_BG_COLOR);
        });
        
        _menuElement = new ControlElement(_menu);
        Disable();
        
        _menu.SetCustomMinimumSize(new Vector2(0, 50));
    }
    protected override List<IFormObject> GetAllElements() => new() { _actionLabel, _leftArrow, _rightArrow, _button, _bgColour };
    protected override void OnDestroy() { }
    
    public ButtonElement GetButton() => _button;
    
    public void SetActionName(string name) => _actionLabel.SetText(name);
    
    private void ShowLeftArrow(bool show) => _leftArrow.SetAlpha(show ? 1.0f : 0.0f);
    private void ShowRightArrow(bool show) => _rightArrow.SetAlpha(show ? 1.0f : 0.0f);

    public void SetStore() {
        SetActionName("Store");
        ShowLeftArrow(false);
        ShowRightArrow(true);
        _bgColour.SetColor(DEFAULT_BG_COLOR);
        _isDisabled = false;
    }
    public void SetTake() {
        SetActionName("Take");
        ShowLeftArrow(true);
        ShowRightArrow(false);
        _bgColour.SetColor(DEFAULT_BG_COLOR);
        _isDisabled = false;
    }
    public void Disable() {
        SetActionName("Select an item");
        ShowLeftArrow(false);
        ShowRightArrow(false);
        _bgColour.SetColor(DISABLED_BG_COLOUR);
        _isDisabled = true;
    }
}