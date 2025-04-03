using System.Collections.Generic;
using Godot;

public class HotbarIcon : FormBase {

    private readonly TextureRectElement _itemIcon;
    private readonly NinePatchRectElement _highlight;
    
    private const string
        FORM_PATH = "res://Main/Prefabs/UI/GameElements/HotbarIcon.tscn",
        ITEM_ICON = "ItemIcon",
        HIGHLIGHT_RECT = "Highlight";

    public HotbarIcon() : base("item_icon", FORM_PATH) {
        TextureRect itemIcon = FindNode<TextureRect>(ITEM_ICON);
        NinePatchRect highlight = FindNode<NinePatchRect>(HIGHLIGHT_RECT);
        
        _itemIcon = new TextureRectElement(itemIcon);
        _highlight = new NinePatchRectElement(highlight);
        
        _highlight.GetElement().SetModulate(Colors.DarkGoldenrod);
        _highlight.SetAlpha(0.0f);
        
        _itemIcon.SetAlpha(0.0f);
        
        _menuElement = new ControlElement(_menu);
    }
    protected override List<IFormObject> GetAllElements() => new() { _itemIcon, _highlight };
    protected override void OnDestroy() { }
    
    public void Highlight(bool highlight) => _highlight.SetAlpha(highlight ? 1.0f : 0.0f);
    public void SetIcon(ItemType item) {
        if (item == null) {
            _itemIcon.SetAlpha(0.0f);
            return;
        }
        _itemIcon.SetAlpha(1.0f);
        _itemIcon.SetTexture(item.GetImagePath());
    }
}