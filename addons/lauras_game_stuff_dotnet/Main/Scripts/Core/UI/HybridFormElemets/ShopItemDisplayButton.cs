
using System;
using System.Collections.Generic;
using Godot;
using NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI.FormElements.Containers;

public class ShopItemDisplayButton : FormBase, IFocusable {
    
    private readonly LabelElement _nameLabel, _costLabel;
    private readonly TextureRectElement _objTexture;
    private readonly ColorRectElement _bgColor;
    private readonly ButtonElement _button;
    
    private readonly ItemType _itemType;
    
    private static readonly Color
        DEFAULT_BG_COLOR = Colors.Black,
        FOCUS_BG_COLOR = Colors.DimGray,
        SELECTED_BG_COLOR = Colors.Gold;
    
    private Action<Key, ShopItemDisplayButton> _keyboardBehaviour;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ShopItemDisplay.tscn",
        OBJ_NAME_LABEL = "Content/ObjName",
        OBJ_COST_LABEL = "Content/ObjCost",
        OBJ_IMG_TEXTURE = "Content/ObjImg",
        BUTTON = "Button",
        BG_COLOUR = "BGContainer/BGColour",
        CREDITS_SYMBOL = " \u20bd";

    public ShopItemDisplayButton(ItemType itemType) : this(itemType.GetItemName()) => _itemType = itemType;

    public ShopItemDisplayButton(string formName, Action<Key, ShopItemDisplayButton> keyboardBehaviour = null) : base(formName, FORM_PATH) {
        Label nameLabel = FindNode<Label>(OBJ_NAME_LABEL);
        Label costLabel = FindNode<Label>(OBJ_COST_LABEL);
        TextureRect textureRect = FindNode<TextureRect>(OBJ_IMG_TEXTURE);
        ColorRect bgColor = FindNode<ColorRect>(BG_COLOUR);
        Button button = FindNode<Button>(BUTTON);
        
        _keyboardBehaviour = keyboardBehaviour;
        
        _nameLabel = new LabelElement(nameLabel);
        _costLabel = new LabelElement(costLabel);
        _objTexture = new TextureRectElement(textureRect);
        _bgColor = new ColorRectElement(bgColor);
        _button = new ButtonElement(button);
        
        _button.AddAction(Control.SignalName.FocusEntered, _ => _bgColor.SetColor(FOCUS_BG_COLOR));
        _button.AddAction(Control.SignalName.FocusExited, _ => _bgColor.SetColor(DEFAULT_BG_COLOR));
        _button.AddAction(Control.SignalName.MouseEntered, _ => GrabFocus());
        
        _menuElement = new ControlElement(_menu);
    }

    protected override List<IFormObject> GetAllElements() => new() { _nameLabel, _costLabel, _objTexture, _bgColor, _button };
    protected override void OnDestroy() { }

    public LabelElement GetNameLabel() => _nameLabel;
    public LabelElement GetCostLabel() => _costLabel;
    public TextureRectElement GetObjTexture() => _objTexture;
    public ColorRectElement GetBgColor() => _bgColor;
    public ButtonElement GetButton() => _button;
    
    public void SetName(string name) => _nameLabel.GetElement().SetText(name);
    public void SetCost(int cost) => _costLabel.GetElement().SetText(cost + CREDITS_SYMBOL);
    public void SetTexture(Texture2D texture) => _objTexture.SetTexture(texture);
    public void SetTexture(string path) => _objTexture.SetTexture(path);
    public void SetBgColor(Color color) => _bgColor.SetColor(color);
    public void SetBgColor(float r, float g, float b, float a) => _bgColor.SetColor(r, g, b, a);
    public void SetHeight(int height) {
        GetMenu().SetCustomMinimumSize(new Vector2(0, height));
        GetObjTexture().SetSize(new Vector2(height, height));
    }

    public ItemType GetItemType() {
        if (_itemType == null) throw new NullReferenceException("ERROR: ShopItemDisplayButton.GetItemType() : Button was not created with an ItemType");
        return _itemType;
    }

    public void OnPressed(Action<ShopItemDisplayButton> onPressed) => _button.OnPressed(_ => onPressed(this));
    
    public void GrabFocus() {
        if (!IsValid() || HasFocus()) return;
        GetButton().GetElement().GrabFocus();
    }

    public void ReleaseFocus() => GetButton().GetElement().ReleaseFocus();
    public bool HasFocus() => IsValid() && GetButton().GetElement().HasFocus();
    public Control GetFocusableElement() => GetButton().GetElement();
    public void VisualPress(bool pressed) => GetBgColor().SetColor(pressed ? SELECTED_BG_COLOR : FOCUS_BG_COLOR);
}