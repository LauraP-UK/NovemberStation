using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class InvItemDisplay : FormBase, IFocusable {
    private readonly LabelElement _nameLabel, _itemCount, _totalWeight;
    private readonly TextureRectElement _itemIcon;
    private readonly ColorRectElement _bgColor;
    private readonly ButtonElement _focusBtn;
    private readonly ControlElement _extraInfoControl;
    private readonly VBoxContainerElement _subListVBox;

    private readonly ItemType _itemType;
    private readonly InventoryForm _ownerForm;
    private readonly List<string> _itemJsons = new();

    private int _count;
    private float _weight;
    private bool _isSelected, _isExpanded;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/InvItemDisplay.tscn",
        ITEM_NAME_LABEL = "Main/Content/ObjName",
        ITEM_COUNT_LABEL = "Main/Content/ObjCount",
        TOTAL_WEIGHT_LABEL = "Main/Content/ObjWeight",
        ITEM_IMG_TEXTURE = "Main/Content/ObjImg",
        BUTTON = "Main/Content/FocusButton",
        EXTRA_INFO = "Main/ExtraInfo",
        SUB_BUTTON_LIST = "Main/ExtraInfo/SubButtonList",
        BG_COLOUR = "Main/BGContainer/BGColour";

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
        Control extraInfo = FindNode<Control>(EXTRA_INFO);
        VBoxContainer subButtonList = FindNode<VBoxContainer>(SUB_BUTTON_LIST);

        _nameLabel = new LabelElement(nameLabel);
        _itemCount = new LabelElement(itemCount);
        _totalWeight = new LabelElement(totalWeight);
        _itemIcon = new TextureRectElement(itemIcon);
        _bgColor = new ColorRectElement(bgColor);
        _focusBtn = new ButtonElement(button);
        _extraInfoControl = new ControlElement(extraInfo);
        _subListVBox = new VBoxContainerElement(subButtonList);

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
        
        OnPressed(_ => {
            SetExpanded(true);
            SelectAndAlert(true, null);
            VisualPress(true);
        });

        _menuElement = new ControlElement(_menu, _ => {
            GetCountLabel().SetText($"x{_count}");
            GetWeightLabel().SetText($"{Mathsf.Round(_weight, 2)} {WEIGHT_SYMBOL}");

            _isExpanded = false;
            _extraInfoControl.GetElement().SetVisible(false);

            List<string> itemJsons = GetItemJsons();
            for (int i = 0; i < itemJsons.Count; i++) {
                string json = itemJsons[i];
                InvItemSummary summary = new(i + 1, json, new Vector2(_menu.GetSize().X, 40), this);
                _subListVBox.AddChild(summary);
            }
        });

        _nameLabel.SetText(item.GetItemName());
        _itemIcon.SetTexture(item.GetImage());

        _menu.SetCustomMinimumSize(new Vector2(0, 50));
    }

    protected override List<IFormObject> GetAllElements() => new() { _nameLabel, _itemCount, _totalWeight, _itemIcon, _bgColor, _focusBtn, _extraInfoControl, _subListVBox };

    protected override void OnDestroy() => _subListVBox.Destroy();

    public LabelElement GetNameLabel() => _nameLabel;
    public LabelElement GetCountLabel() => _itemCount;
    public LabelElement GetWeightLabel() => _totalWeight;
    public TextureRectElement GetItemTexture() => _itemIcon;
    public ColorRectElement GetBgColor() => _bgColor;
    public ButtonElement GetButton() => _focusBtn;
    public ItemType GetItemType() => _itemType;
    public VBoxContainerElement GetSubListVBox() => _subListVBox;
    public void AddCount(int count) => _count += count;
    public int GetCount() => _count;
    public void AddWeight(float weight) => _weight += weight;

    public void AddItemJson(string itemJson) {
        _itemJsons.Add(itemJson);
        _subListVBox?.GetNode().SetCustomMinimumSize(new Vector2(_menu.GetSize().X, 50 + (GetItemJsons().Count) * 40));
    }

    public List<string> GetItemJsons() => _itemJsons;

    public List<InvItemSummary> GetSummaries() => GetSubListVBox().GetDisplayObjects().Cast<InvItemSummary>().ToList();

    public void OnPressed(Action<InvItemDisplay> onPressed) => _focusBtn.OnPressed(_ => onPressed(this));

    public void SelectAndAlert(bool selected, InvItemSummary selectedSummary) {
        Select(selected);
        if (selected) _ownerForm.SetSelectedItem(this, selectedSummary);
    }
    public void Select(bool selected) {
        _isSelected = selected;
        GetBgColor().SetColor(selected ? SELECTED_BG_COLOR : DEFAULT_BG_COLOR);
    }

    public bool IsSelected() => _isSelected;

    public void SetExpanded(bool expanded) {
        _isExpanded = expanded;
        _extraInfoControl.GetElement().SetVisible(expanded);

        if (expanded) {
            _extraInfoControl.GetNode().QueueRedraw();
            _menu.SetCustomMinimumSize(new Vector2(0, 50 + (GetItemJsons().Count) * 40));
        }
        else {
            _extraInfoControl.GetNode().SetCustomMinimumSize(Vector2.Zero);
            _extraInfoControl.GetNode().QueueRedraw();
            _menu.SetCustomMinimumSize(new Vector2(0, 50));
        }
        _menu.QueueRedraw();
    }

    public bool IsExpanded() => _isExpanded;
    
    public string GetFirstJson() => _itemJsons[0];

    public string GetJsonFromExpanded() {
        if (IsExpanded()) {
            List<InvItemSummary> subBtns = _subListVBox.GetDisplayObjects().Cast<InvItemSummary>().ToList();
            InvItemSummary selected = subBtns.FirstOrDefault(btn => btn.IsSelected());
            if (selected != null) return selected.GetJson();
        }

        return GetItemJsons()[0];
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