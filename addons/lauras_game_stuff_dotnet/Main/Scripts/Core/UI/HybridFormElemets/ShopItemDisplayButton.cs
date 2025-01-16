
using System;
using System.Collections.Generic;
using Godot;

public class ShopItemDisplayButton : FormBase, IFocusable {
    
    private readonly LabelElement _nameLabel, _costLabel;
    private readonly TextureRectElement _objTexture;
    private readonly ColorRectElement _bgColor;
    private readonly ButtonElement _button;
    
    private static readonly Color
        DEFAULT_BG_COLOR = Colors.Black,
        FOCUS_BG_COLOR = Colors.DimGray,
        SELECTED_BG_COLOR = Colors.Red;
    
    private Action<Key, ShopItemDisplayButton> _keyboardBehaviour;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/ShopItemDisplay.tscn",
        OBJ_NAME_LABEL = "Content/ObjName",
        OBJ_COST_LABEL = "Content/ObjCost",
        OBJ_IMG_TEXTURE = "Content/ObjImg",
        BUTTON = "Button",
        BG_COLOUR = "BGContainer/BGColour",
        CREDITS_SYMBOL = " \u20bd";

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
        //_button.AddAction(Control.SignalName.MouseExited, _ => _bgColor.SetColor(DEFAULT_BG_COLOR));
        
        _menuLayout = new ControlLayout(_menu, _ => _menuLayout.ConnectSignals());
        _menuLayout.Build();
    }

    protected override List<IFormObject> getAllElements() => new() { _nameLabel, _costLabel, _objTexture, _bgColor };
    
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
    public void SetHeight(int height) => GetMenu().SetCustomMinimumSize(new Vector2(0, height));
    
    public string GetName() => _nameLabel.GetElement().GetText();
    public int GetCost() => int.Parse(_costLabel.GetElement().GetText().Replace(CREDITS_SYMBOL, ""));
    
    public void OnPressed(Action<ShopItemDisplayButton> onPressed) => _button.OnPressed(_ => onPressed(this));
    
    public void GrabFocus() => GetButton().GetElement().GrabFocus();

    public bool HasFocus() => GetButton().GetElement().HasFocus();
    public void VisualPress(bool pressed) => GetBgColor().SetColor(pressed ? SELECTED_BG_COLOR : FOCUS_BG_COLOR);
}