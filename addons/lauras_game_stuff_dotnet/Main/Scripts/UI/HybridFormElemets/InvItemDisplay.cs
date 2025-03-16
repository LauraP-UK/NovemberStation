using System;
using System.Collections.Generic;
using Godot;

public class InvItemDisplay : FormBase, IFocusable {
    private readonly LabelElement _nameLabel, _itemCount, _totalWeight;
    private readonly TextureRectElement _itemIcon;
    private readonly ColorRectElement _bgColor;
    private readonly ButtonElement _button;

    private readonly ItemType _itemType;
    private int _count = 0;
    private float _weight = 0.0f;
    private bool _isSelected = false;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/InvItemDisplay.tscn",
        ITEM_NAME_LABEL = "Content/ObjName",
        ITEM_COUNT_LABEL = "Content/ObjCount",
        TOTAL_WEIGHT_LABEL = "Content/ObjWeight",
        ITEM_IMG_TEXTURE = "Content/ObjImg",
        BUTTON = "FocusButton",
        BG_COLOUR = "BGContainer/BGColour";

    private static readonly Color
        DEFAULT_BG_COLOR = new(0.1f, 0.1f, 0.1f),
        FOCUS_BG_COLOR = Colors.DimGray,
        SELECTED_BG_COLOR = Colors.Gold;

    public const string WEIGHT_SYMBOL = "kg";

    public InvItemDisplay(ItemType item) : base(item.GetTypeID() + "_display_btn", FORM_PATH) {
        _itemType = item;

        Label nameLabel = FindNode<Label>(ITEM_NAME_LABEL);
        Label itemCount = FindNode<Label>(ITEM_COUNT_LABEL);
        Label totalWeight = FindNode<Label>(TOTAL_WEIGHT_LABEL);
        TextureRect itemIcon = FindNode<TextureRect>(ITEM_IMG_TEXTURE);
        ColorRect bgColor = FindNode<ColorRect>(BG_COLOUR);
        Button button = FindNode<Button>(BUTTON);

        _nameLabel = new LabelElement(nameLabel);
        _itemCount = new LabelElement(itemCount);
        _totalWeight = new LabelElement(totalWeight);
        _itemIcon = new TextureRectElement(itemIcon);
        _bgColor = new ColorRectElement(bgColor);
        _button = new ButtonElement(button);

        _bgColor.SetColor(DEFAULT_BG_COLOR);

        _button.AddAction(Control.SignalName.FocusEntered, _ => {
            if (_isSelected) return;
            GetBgColor().SetColor(FOCUS_BG_COLOR);
        });
        _button.AddAction(Control.SignalName.FocusExited, _ => {
            if (_isSelected) return;
            GetBgColor().SetColor(DEFAULT_BG_COLOR);
        });
        _button.AddAction(Control.SignalName.MouseEntered, _ => {
            if (_isSelected) return;
            GrabFocus();
        });
        
        _button.OnButtonDown(_ => VisualPress(true));
        _button.OnButtonUp(_ => {
            if (_isSelected) return;
            VisualPress(false);
        });

        _menuElement = new ControlElement(_menu, _ => {
            GetCountLabel().SetText($"x{_count}");
            GetWeightLabel().SetText($"{_weight:0.00} {WEIGHT_SYMBOL}");
        });
        
        _nameLabel.SetText(item.GetItemName());
        _itemIcon.SetTexture(item.GetImage());
        
        _menuElement.GetElement().SetCustomMinimumSize(new Vector2(0, 50));
    }

    protected override List<IFormObject> GetAllElements() => new() { _nameLabel, _itemCount, _totalWeight, _itemIcon, _bgColor, _button };
    protected override void OnDestroy() {}

    public LabelElement GetNameLabel() => _nameLabel;
    public LabelElement GetCountLabel() => _itemCount;
    public LabelElement GetWeightLabel() => _totalWeight;
    public TextureRectElement GetItemTexture() => _itemIcon;
    public ColorRectElement GetBgColor() => _bgColor;
    public ButtonElement GetButton() => _button;
    public ItemType GetItemType() => _itemType;
    public void AddCount(int count) => _count += count;
    public void AddWeight(float weight) => _weight += weight;

    public void OnPressed(Action<InvItemDisplay> onPressed) => _button.OnPressed(_ => onPressed(this));
    public void Select(bool selected) {
        _isSelected = selected;
        GetBgColor().SetColor(selected ? SELECTED_BG_COLOR : DEFAULT_BG_COLOR);
    }
    public bool IsSelected() => _isSelected;

    public void GrabFocus() {
        if (!IsValid() || HasFocus()) return;
        GetButton().GrabFocus();
    }

    public void ReleaseFocus() => GetButton().GetElement().ReleaseFocus();
    public bool HasFocus() => IsValid() && GetButton().GetElement().HasFocus();
    public Control GetFocusableElement() => GetButton().GetElement();
    public void VisualPress(bool pressed) => GetBgColor().SetColor(pressed ? SELECTED_BG_COLOR : FOCUS_BG_COLOR);
}