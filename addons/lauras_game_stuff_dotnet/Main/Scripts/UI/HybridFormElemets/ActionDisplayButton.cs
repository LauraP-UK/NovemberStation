
using System.Collections.Generic;
using Godot;

public class ActionDisplayButton : FormBase, IFocusable {

    private readonly LabelElement _numLabel, _nameLabel;
    private readonly ColorRectElement _bgColor;
    private readonly NinePatchRectElement _btnFrame;
    private readonly ButtonElement _focusButton;

    private static readonly Color
        DEFAULT_BG_COLOR = Colors.Black,
        FOCUS_BG_COLOR = Colors.DimGray;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ActionDisplayElement.tscn",
        BG_COLOUR = "BGColour",
        ACTION_NUM = "ActionNum",
        ACTION_NAME = "ActionName",
        BUTTON_FRAME = "BtnFrame",
        FOCUS_BUTTON = "FocusButton";

    public ActionDisplayButton(string formName) : base(formName, FORM_PATH) {
        Label numLabel = FindNode<Label>(ACTION_NUM);
        Label nameLabel = FindNode<Label>(ACTION_NAME);
        ColorRect bgColor = FindNode<ColorRect>(BG_COLOUR);
        NinePatchRect btnFrame = FindNode<NinePatchRect>(BUTTON_FRAME);
        Button focusButton = FindNode<Button>(FOCUS_BUTTON);
        
        _numLabel = new LabelElement(numLabel);
        _nameLabel = new LabelElement(nameLabel);
        _bgColor = new ColorRectElement(bgColor);
        _btnFrame = new NinePatchRectElement(btnFrame);
        _focusButton = new ButtonElement(focusButton);
        
        _focusButton.AddAction(Control.SignalName.FocusEntered, _ => _bgColor.SetColor(FOCUS_BG_COLOR));
        _focusButton.AddAction(Control.SignalName.FocusExited, _ => _bgColor.SetColor(DEFAULT_BG_COLOR));
        
        _menuElement = new ControlElement(_menu);
        SetCaptureInput(false);
        EventManager.UnregisterListeners(this);
    }

    private void SetSize() => GetNode().SetScale(new Vector2(0.1f, 0.1f));

    protected override List<IFormObject> GetAllElements() => new() {_nameLabel, _numLabel, _bgColor, _focusButton};
    protected override void OnDestroy() { }
    
    public void SetActionNum(string num) => _numLabel.GetElement().SetText(num);
    public void SetActionName(string name) => _nameLabel.GetElement().SetText(name);
    public void SetBGColour(Color colour) => _bgColor.SetColor(colour);
    public void SetAlpha(float alpha) {
        _bgColor.SetAlpha(alpha);
        _nameLabel.SetAlpha(alpha);
        _numLabel.SetAlpha(alpha);
        _btnFrame.SetAlpha(alpha);
    }
    
    public float GetAlpha() => _bgColor.GetAlpha();

    public void GrabFocus() {
        if (!IsValid() || HasFocus()) return;
        _focusButton.GrabFocus();
    }

    public void ReleaseFocus() => _focusButton.ReleaseFocus();
    public bool HasFocus() => IsValid() && _focusButton.HasFocus();
    public Control GetFocusableElement() => _focusButton.GetElement();
    public override bool LockMovement() => false;

    public override int GetHashCode() => _menu.Name.GetHashCode();

    public override bool Equals(object obj) {
        if (!IsValid()) return false;
        if (obj is not ActionDisplayButton other) return false;
        return _menu.Name == other._menu.Name;
    }
}