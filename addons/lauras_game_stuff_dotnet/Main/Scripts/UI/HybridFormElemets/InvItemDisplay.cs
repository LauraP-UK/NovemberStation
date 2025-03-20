using System;
using System.Collections.Generic;
using Godot;

public class InvItemDisplay : FormBase, IFocusable {
    private readonly LabelElement _nameLabel, _itemCount, _totalWeight;
    private readonly TextureRectElement _itemIcon;
    private readonly ColorRectElement _bgColor;
    private readonly ButtonElement _focusBtn, _expandBtn;
    private readonly ControlElement _extraInfoControl;
    private readonly ScrollDisplayList _subList;

    private readonly ItemType _itemType;
    private readonly InventoryForm _ownerForm;
    private readonly List<string> _itemJsons = new();
    
    private int _count = 0;
    private float _weight = 0.0f;
    private bool _isSelected = false, _isExpanded = false;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/InvItemDisplay.tscn",
        ITEM_NAME_LABEL = "Content/ObjName",
        ITEM_COUNT_LABEL = "Content/ObjCount",
        TOTAL_WEIGHT_LABEL = "Content/ObjWeight",
        ITEM_IMG_TEXTURE = "Content/ObjImg",
        BUTTON = "Content/FocusButton",
        EXPAND_BUTTON = "Content/ExpandButton",
        EXTRA_INFO = "Content/ExtraInfo",
        BG_COLOUR = "BGContainer/BGColour";
    
    public const string
        UP_ARROW = "\u25b2",
        DOWN_ARROW = "\u25bc",
        RIGHT_ARROW = "\u25ba";

    private static readonly Color
        DEFAULT_BG_COLOR = new(0.1f, 0.1f, 0.1f),
        FOCUS_BG_COLOR = Colors.DimGray,
        SELECTED_BG_COLOR = Colors.DarkGoldenrod;

    public const string WEIGHT_SYMBOL = "kg";

    public InvItemDisplay(ItemType item, InventoryForm ownerForm) : base(item.GetTypeID() + "_display_btn", FORM_PATH) {
        _itemType = item;
        _ownerForm = ownerForm;

        Label nameLabel = FindNode<Label>(ITEM_NAME_LABEL);
        Label itemCount = FindNode<Label>(ITEM_COUNT_LABEL);
        Label totalWeight = FindNode<Label>(TOTAL_WEIGHT_LABEL);
        TextureRect itemIcon = FindNode<TextureRect>(ITEM_IMG_TEXTURE);
        ColorRect bgColor = FindNode<ColorRect>(BG_COLOUR);
        Button button = FindNode<Button>(BUTTON);
        Button expandBtn = FindNode<Button>(EXPAND_BUTTON);
        Control extraInfo = FindNode<Control>(EXTRA_INFO);

        _nameLabel = new LabelElement(nameLabel);
        _itemCount = new LabelElement(itemCount);
        _totalWeight = new LabelElement(totalWeight);
        _itemIcon = new TextureRectElement(itemIcon);
        _bgColor = new ColorRectElement(bgColor);
        _focusBtn = new ButtonElement(button);
        _expandBtn = new ButtonElement(expandBtn);
        _extraInfoControl = new ControlElement(extraInfo);

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
        _focusBtn.OnButtonDown(_ => VisualPress(true));
        _focusBtn.OnButtonUp(_ => {
            if (_isSelected) return;
            VisualPress(false);
        });
        
        _expandBtn.OnPressed(_ => {
            SetExpanded(!IsExpanded());
            GD.Print($"Toggle expanded: {IsExpanded()}");
            _ownerForm.RefreshFromButton(this);
        });
        _subList = new ScrollDisplayList(_menu.Name+"_sublist");
        _extraInfoControl.GetElement().AddChild(_subList.GetNode());

        _menuElement = new ControlElement(_menu, _ => {
            GetCountLabel().SetText($"x{_count}");
            GetWeightLabel().SetText($"{Mathsf.Round(_weight, 2)} {WEIGHT_SYMBOL}");
            
            _isExpanded = false;
            _expandBtn.GetElement().SetText("+");
            _extraInfoControl.GetElement().SetVisible(false);
            
            List<string> itemJsons = GetItemJsons();
            if (itemJsons.Count <= 1) _expandBtn.GetElement().Hide();
            else {
                for (int i = 0; i < itemJsons.Count; i++) {
                    string json = itemJsons[i];
                    InvItemSummary summary = new(i + 1, json, new Vector2(_menu.GetSize().X, 40));
                    _subList.AddElement(summary);
                }
            }
        });
        
        _nameLabel.SetText(item.GetItemName());
        _itemIcon.SetTexture(item.GetImage());
        
        _menu.SetCustomMinimumSize(new Vector2(0, 50));
    }

    protected override List<IFormObject> GetAllElements() => new() { _nameLabel, _itemCount, _totalWeight, _itemIcon, _bgColor, _focusBtn };
    protected override void OnDestroy() {}

    public LabelElement GetNameLabel() => _nameLabel;
    public LabelElement GetCountLabel() => _itemCount;
    public LabelElement GetWeightLabel() => _totalWeight;
    public TextureRectElement GetItemTexture() => _itemIcon;
    public ColorRectElement GetBgColor() => _bgColor;
    public ButtonElement GetButton() => _focusBtn;
    public ItemType GetItemType() => _itemType;
    public void AddCount(int count) => _count += count;
    public int GetCount() => _count;
    public void AddWeight(float weight) => _weight += weight;
    public void AddItemJson(string itemJson) {
        _itemJsons.Add(itemJson);
        _subList.GetNode().SetSize(new Vector2(_menu.GetSize().X, (GetItemJsons().Count) * 40));
    }

    public List<string> GetItemJsons() => _itemJsons;

    public void OnPressed(Action<InvItemDisplay> onPressed) => _focusBtn.OnPressed(_ => onPressed(this));
    public void Select(bool selected) {
        _isSelected = selected;
        GetBgColor().SetColor(selected ? SELECTED_BG_COLOR : DEFAULT_BG_COLOR);
    }
    public bool IsSelected() => _isSelected;
    
    public void SetExpanded(bool expanded) {
        _isExpanded = expanded;
        _expandBtn.GetElement().SetText(expanded ? "-" : "+");
        _extraInfoControl.GetElement().SetVisible(expanded);
    }
    public bool IsExpanded() => _isExpanded;

    public void GrabFocus() {
        if (!IsValid() || HasFocus()) return;
        GetButton().GrabFocus();
    }

    public void ReleaseFocus() => GetButton().GetElement().ReleaseFocus();
    public bool HasFocus() => IsValid() && GetButton().GetElement().HasFocus();
    public Control GetFocusableElement() => GetButton().GetElement();
    public void VisualPress(bool pressed) => GetBgColor().SetColor(pressed ? SELECTED_BG_COLOR : FOCUS_BG_COLOR);
}