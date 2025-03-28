using System.Collections.Generic;
using System.Linq;
using Godot;

public class InvItemSummary : FormBase, IFocusable {
    private readonly LabelElement _numberLabel, _summaryLabel;
    private readonly ColorRectElement _bgColor;
    private readonly ButtonElement _focusBtn;
    private readonly NinePatchRectElement _highlightRect;

    private readonly InvItemDisplay _owningBtn;
    
    private readonly string _json;
    private bool _isSelected;
    private int _index;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/InvItemSummary.tscn",
        NUMBER_LABEL = "Content/ObjNumber",
        SUMMARY_LABEL = "Content/ObjName",
        BG_COLOUR = "BGContainer/BGColour",
        FOCUS_BUTTON = "Content/FocusButton",
        HIGHLIGHT_RECT = "BGContainer/Highlight";

    private static readonly Color
        DEFAULT_BG_COLOR = ColourHelper.GetFrom255(46,46,46,1.0f),
        FOCUS_BG_COLOR = Colors.DimGray,
        SELECTED_BG_COLOR = Colors.DarkGoldenrod;

    public InvItemSummary(int index, string json, Vector2 size, InvItemDisplay owningBtn) : base("summary_button_"+index, FORM_PATH) {
        _json = json;
        _owningBtn = owningBtn;
        _index = index;
        
        Label numberLabel = FindNode<Label>(NUMBER_LABEL);
        Label summaryLabel = FindNode<Label>(SUMMARY_LABEL);
        ColorRect bgColor = FindNode<ColorRect>(BG_COLOUR);
        Button focusBtn = FindNode<Button>(FOCUS_BUTTON);
        NinePatchRect highlight = FindNode<NinePatchRect>(HIGHLIGHT_RECT);
        
        _numberLabel = new LabelElement(numberLabel);
        _summaryLabel = new LabelElement(summaryLabel);
        _bgColor = new ColorRectElement(bgColor);
        _focusBtn = new ButtonElement(focusBtn);
        _highlightRect = new NinePatchRectElement(highlight);
        
        _highlightRect.SetAlpha(0.0f);
        _bgColor.SetColor(DEFAULT_BG_COLOR);
        
        _focusBtn.AddAction(Control.SignalName.FocusEntered, _ => {
            if (_isSelected) return;
            GetBgColor().SetColor(FOCUS_BG_COLOR);
        });
        _focusBtn.AddAction(Control.SignalName.FocusExited, _ => {
            if (_isSelected) return;
            GetBgColor().SetColor(DEFAULT_BG_COLOR);
        });
        _focusBtn.AddAction(Control.SignalName.MouseEntered, _ => {
            if (_isSelected) return;
            GrabFocus();
        });
        _focusBtn.OnButtonDown(_ => {
            GrabFocus();
            List<InvItemSummary> btns = _owningBtn.GetSubListVBox().GetDisplayObjects().Cast<InvItemSummary>().ToList();
            foreach (InvItemSummary btn in btns) btn.SetSelected(false);
            SelectAndAlert(true);
            VisualPress(true);
        });
        _focusBtn.OnButtonUp(_ => {
            if (_isSelected) return;
            VisualPress(false);
        });
        
        _numberLabel.SetText($"{index}:");
        IObjectBase obj = ObjectAtlas.DeserialiseDataWithoutNode(json);
        Serialiser.ObjectSaveData data = ObjectAtlas.DeserialiseObject(json);
        obj.BuildFromData(data.Data);
        string summary = obj.GetSummary();
        _summaryLabel.SetText(summary == "" ? obj.GetDisplayName() : summary);
        
        _menu.SetCustomMinimumSize(size);
    }
    protected override List<IFormObject> GetAllElements() => new () {_numberLabel, _summaryLabel , _bgColor, _focusBtn };
    protected override void OnDestroy() { }
    public ButtonElement GetButton() => _focusBtn;
    public ColorRectElement GetBgColor() => _bgColor;
    public string GetJson() => _json;
    public int GetIndex() => _index;

    public void SelectAndAlert(bool selected) {
        SetSelected(selected);
        if (selected) _owningBtn.SelectAndAlert(true, this);
    }
    public void SetSelected(bool selected) {
        _isSelected = selected;
        GetBgColor().SetColor(selected ? SELECTED_BG_COLOR : DEFAULT_BG_COLOR);
    }
    public bool IsSelected() => _isSelected;
    public void Highlight(bool highlight) {
        GetBgColor().SetColor(DEFAULT_BG_COLOR);
        _highlightRect.SetAlpha(highlight ? 0.5f : 0.0f);
    }

    public void GrabFocus() {
        if (!IsValid() || HasFocus()) return;
        GetButton().GrabFocus();
    }

    public void ReleaseFocus() => GetButton().GetElement().ReleaseFocus();
    public bool HasFocus() => IsValid() && GetButton().GetElement().HasFocus();
    public Control GetFocusableElement() => GetButton().GetElement();
    public void VisualPress(bool pressed) => GetBgColor().SetColor(pressed ? SELECTED_BG_COLOR : FOCUS_BG_COLOR);
}