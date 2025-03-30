using System.Collections.Generic;
using Godot;

public class HotbarItem : FormBase {
    private readonly ButtonElement _removeButton, _moveUpButton, _moveDownButton;
    private readonly ColorRectElement _removeBG, _moveUpBG, _moveDownBG;
    private readonly TextureRectElement _itemIcon;
    
    private readonly string _json;

    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/HotbarControls.tscn",
        REMOVE_BUTTON = "HBoxContainer/XButtonContainer/FocusBtn",
        MOVE_UP_BUTTON = "HBoxContainer/VBoxContainer/Up/FocusBtn",
        MOVE_DOWN_BUTTON = "HBoxContainer/VBoxContainer/Down/FocusBtn",
        REMOVE_BG = "HBoxContainer/XButtonContainer/XBG",
        MOVE_UP_BG = "HBoxContainer/VBoxContainer/Up/UpBG",
        MOVE_DOWN_BG = "HBoxContainer/VBoxContainer/Down/DownBG",
        ITEM_ICON = "HBoxContainer/Icon/ObjImg";

    private readonly Color
        DEFAULT_BG = ColourHelper.GetFrom255(55, 52, 50, 1.0f),
        HOVER_BG = Colors.DimGray,
        CLICK_BG = Colors.DarkGoldenrod;

    public HotbarItem(string json, ItemType overrideType = null) : base("hotbar_item", FORM_PATH) {
        _json = json;
        
        Button removeBtn = FindNode<Button>(REMOVE_BUTTON);
        Button moveUpBtn = FindNode<Button>(MOVE_UP_BUTTON);
        Button moveDownBtn = FindNode<Button>(MOVE_DOWN_BUTTON);
        ColorRect removeBG = FindNode<ColorRect>(REMOVE_BG);
        ColorRect moveUpBG = FindNode<ColorRect>(MOVE_UP_BG);
        ColorRect moveDownBG = FindNode<ColorRect>(MOVE_DOWN_BG);
        TextureRect itemIcon = FindNode<TextureRect>(ITEM_ICON);

        _removeButton = new ButtonElement(removeBtn);
        _moveUpButton = new ButtonElement(moveUpBtn);
        _moveDownButton = new ButtonElement(moveDownBtn);
        _removeBG = new ColorRectElement(removeBG);
        _moveUpBG = new ColorRectElement(moveUpBG);
        _moveDownBG = new ColorRectElement(moveDownBG);
        _itemIcon = new TextureRectElement(itemIcon);

        _menuElement = new ControlElement(_menu);

        _removeButton.OnMouseEntered(_ => _removeBG.SetColor(HOVER_BG));
        _removeButton.OnMouseExited(_ => _removeBG.SetColor(DEFAULT_BG));
        
        _moveUpButton.OnMouseEntered(_ => _moveUpBG.SetColor(HOVER_BG));
        _moveUpButton.OnMouseExited(_ => _moveUpBG.SetColor(DEFAULT_BG));
        
        _moveDownButton.OnMouseEntered(_ => _moveDownBG.SetColor(HOVER_BG));
        _moveDownButton.OnMouseExited(_ => _moveDownBG.SetColor(DEFAULT_BG));
        
        _removeButton.OnButtonDown(_ => _removeBG.SetColor(CLICK_BG));
        _removeButton.OnButtonUp(_ => _removeBG.SetColor(HOVER_BG));
        _moveUpButton.OnButtonDown(_ => _moveUpBG.SetColor(CLICK_BG));
        _moveUpButton.OnButtonUp(_ => _moveUpBG.SetColor(HOVER_BG));
        _moveDownButton.OnButtonDown(_ => _moveDownBG.SetColor(CLICK_BG));
        _moveDownButton.OnButtonUp(_ => _moveDownBG.SetColor(HOVER_BG));

        string itemID = overrideType == null ? Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, json) : "";
        ItemType item = overrideType ?? Items.GetViaID(itemID);
        
        _itemIcon.SetTexture(item.GetImagePath());
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
}