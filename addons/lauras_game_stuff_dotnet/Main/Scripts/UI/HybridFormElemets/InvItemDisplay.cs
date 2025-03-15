using System;
using System.Collections.Generic;
using Godot;

public class InvItemDisplay : FormBase, IFocusable {
    private readonly LabelElement _nameLabel, _itemCount, _totalWeight;
    private readonly TextureRectElement _itemIcon;
    private readonly ColorRectElement _bgColor;
    private readonly ButtonElement _button;

    private readonly ItemType _itemType;

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

        _button.AddAction(Control.SignalName.FocusEntered, _ => _bgColor.SetColor(FOCUS_BG_COLOR));
        _button.AddAction(Control.SignalName.FocusExited, _ => _bgColor.SetColor(DEFAULT_BG_COLOR));
        _button.AddAction(Control.SignalName.MouseEntered, _ => GrabFocus());

        _menuElement = new ControlElement(_menu);
        
        _nameLabel.SetText(item.GetItemName());
        _itemIcon.SetTexture(item.GetImage());
    }

    protected override List<IFormObject> GetAllElements() => new() { _nameLabel, _itemCount, _totalWeight, _itemIcon, _bgColor, _button };
    protected override void OnDestroy() {}

    public LabelElement GetNameLabel() => _nameLabel;
    public LabelElement GetCountLabel() => _itemCount;
    public LabelElement GetWeightLabel() => _totalWeight;
    public TextureRectElement GetItemTexture() => _itemIcon;
    public ColorRectElement GetBgColor() => _bgColor;
    public ButtonElement GetButton() => _button;
    public void SetWeight(float weight) => _totalWeight.SetText($"{weight:000.00} {WEIGHT_SYMBOL}");
    public void SetCount(int count) => _itemCount.SetText($"x{count}");

    public void OnPressed(Action<InvItemDisplay> onPressed) => _button.OnPressed(_ => onPressed(this));

    public void GrabFocus() {
        if (!IsValid() || HasFocus()) return;
        GetButton().GetElement().GrabFocus();
    }

    public void ReleaseFocus() => GetButton().GetElement().ReleaseFocus();
    public bool HasFocus() => IsValid() && GetButton().GetElement().HasFocus();
    public Control GetFocusableElement() => GetButton().GetElement();
    public void VisualPress(bool pressed) => GetBgColor().SetColor(pressed ? SELECTED_BG_COLOR : FOCUS_BG_COLOR);
}