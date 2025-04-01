using System;
using System.Collections.Generic;
using Godot;

public class HotbarItem : FormBase {
    private readonly ButtonElement _removeButton, _moveUpButton, _moveDownButton;
    private readonly ColorRectElement _removeBG, _moveUpBG, _moveDownBG;
    private readonly TextureRectElement _itemIcon;
    private readonly ControlElement _removeContainer, _upContainer, _downContainer;
    private readonly VBoxContainerElement _arrowsContainer;

    private readonly SingleInventoryForm _form;
    private readonly int _index;

    private string _json;
    private IContainer _owner;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/HotbarControls.tscn",
        REMOVE_BUTTON = "HBoxContainer/XButtonContainer/FocusBtn",
        MOVE_UP_BUTTON = "HBoxContainer/VBoxContainer/Up/FocusBtn",
        MOVE_DOWN_BUTTON = "HBoxContainer/VBoxContainer/Down/FocusBtn",
        REMOVE_BG = "HBoxContainer/XButtonContainer/XBG",
        MOVE_UP_BG = "HBoxContainer/VBoxContainer/Up/UpBG",
        MOVE_DOWN_BG = "HBoxContainer/VBoxContainer/Down/DownBG",
        ITEM_ICON = "HBoxContainer/Icon/ObjImg",
        REMOVE_CONTAINER = "HBoxContainer/XButtonContainer",
        UP_CONTAINER = "HBoxContainer/VBoxContainer/Up",
        DOWN_CONTAINER = "HBoxContainer/VBoxContainer/Down",
        ARROWS_CONTAINER = "HBoxContainer/VBoxContainer";

    private readonly Color
        DEFAULT_BG = ColourHelper.GetFrom255(55, 52, 50, 1.0f),
        HOVER_BG = Colors.DimGray,
        CLICK_BG = Colors.DarkGoldenrod;

    public HotbarItem(int index, SingleInventoryForm form) : base("hotbar_item", FORM_PATH) {
        _index = index;
        _form = form;

        Button removeBtn = FindNode<Button>(REMOVE_BUTTON);
        Button moveUpBtn = FindNode<Button>(MOVE_UP_BUTTON);
        Button moveDownBtn = FindNode<Button>(MOVE_DOWN_BUTTON);
        ColorRect removeBG = FindNode<ColorRect>(REMOVE_BG);
        ColorRect moveUpBG = FindNode<ColorRect>(MOVE_UP_BG);
        ColorRect moveDownBG = FindNode<ColorRect>(MOVE_DOWN_BG);
        TextureRect itemIcon = FindNode<TextureRect>(ITEM_ICON);
        Control removeContainer = FindNode<Control>(REMOVE_CONTAINER);
        Control upContainer = FindNode<Control>(UP_CONTAINER);
        Control downContainer = FindNode<Control>(DOWN_CONTAINER);
        VBoxContainer arrowsContainer = FindNode<VBoxContainer>(ARROWS_CONTAINER);

        _removeButton = new ButtonElement(removeBtn);
        _moveUpButton = new ButtonElement(moveUpBtn);
        _moveDownButton = new ButtonElement(moveDownBtn);
        _removeBG = new ColorRectElement(removeBG);
        _moveUpBG = new ColorRectElement(moveUpBG);
        _moveDownBG = new ColorRectElement(moveDownBG);
        _itemIcon = new TextureRectElement(itemIcon);
        _removeContainer = new ControlElement(removeContainer);
        _upContainer = new ControlElement(upContainer);
        _downContainer = new ControlElement(downContainer);
        _arrowsContainer = new VBoxContainerElement(arrowsContainer);

        _menuElement = new ControlElement(_menu);

        _removeButton.OnMouseEntered(_ => _removeBG.SetColor(HOVER_BG));
        _removeButton.OnMouseExited(_ => _removeBG.SetColor(DEFAULT_BG));
        _removeButton.OnButtonDown(_ => _removeBG.SetColor(CLICK_BG));
        _removeButton.OnButtonUp(_ => _removeBG.SetColor(HOVER_BG));
        _removeButton.OnPressed(
            _ => {
                if (GetOwner() is not Player player) return;
                player.GetHotbar().RemoveFromHotbar(_index);
                form.RefreshHotbarIcons();
            }
        );

        _moveUpButton.OnMouseEntered(_ => _moveUpBG.SetColor(HOVER_BG));
        _moveUpButton.OnMouseExited(_ => _moveUpBG.SetColor(DEFAULT_BG));
        _moveUpButton.OnButtonDown(_ => _moveUpBG.SetColor(CLICK_BG));
        _moveUpButton.OnButtonUp(_ => _moveUpBG.SetColor(HOVER_BG));
        _moveUpButton.OnPressed(
            _ => {
                if (GetOwner() is not Player player) return;
                player.GetHotbar().MoveHotbarItem(_index, true);
                form.RefreshHotbarIcons();
            }
        );

        _moveDownButton.OnMouseEntered(_ => _moveDownBG.SetColor(HOVER_BG));
        _moveDownButton.OnMouseExited(_ => _moveDownBG.SetColor(DEFAULT_BG));
        _moveDownButton.OnButtonDown(_ => _moveDownBG.SetColor(CLICK_BG));
        _moveDownButton.OnButtonUp(_ => _moveDownBG.SetColor(HOVER_BG));
        _moveDownButton.OnPressed(
            _ => {
                if (GetOwner() is not Player player) return;
                player.GetHotbar().MoveHotbarItem(_index, false);
                form.RefreshHotbarIcons();
            }
        );

        _arrowsContainer.SetAlignment(BoxContainer.AlignmentMode.Center);

        SetFromItem("");
    }

    protected override List<IFormObject> GetAllElements() => new() {
        _removeButton,
        _moveUpButton,
        _moveDownButton,
        _removeBG,
        _moveUpBG,
        _moveDownBG,
        _itemIcon
    };

    protected override void OnDestroy() { }
    public int GetIndex() => _index;
    public void SetOwner(IContainer owner) => _owner = owner;

    private IContainer GetOwner() {
        if (_owner == null) throw new NullReferenceException("Owner is null. Set the owner before using this method!");
        return _owner;
    }

    public void SetFromItem(string json) {
        _json = json;

        if (json == "") {
            _itemIcon.ClearTexture();
            _itemIcon.GetElement().SetVisible(false);
            _removeContainer.GetElement().SetVisible(false);
            _upContainer.GetElement().SetVisible(false);
            _downContainer.GetElement().SetVisible(false);
            return;
        }

        string itemTag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, json);
        ItemType itemType = Items.GetViaID(itemTag);
        _itemIcon.SetTexture(itemType.GetImagePath());
        _itemIcon.GetElement().SetVisible(true);
        _removeContainer.GetElement().SetVisible(true);

        (bool up, bool down) = GameManager.I().GetPlayer().GetHotbar().GetHotbarItemMovement(GetIndex());

        _upContainer.GetElement().SetVisible(up);
        _downContainer.GetElement().SetVisible(down);
    }
}