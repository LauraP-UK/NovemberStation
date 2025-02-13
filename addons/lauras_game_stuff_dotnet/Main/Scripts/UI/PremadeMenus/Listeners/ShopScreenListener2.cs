
using Godot;

public class ShopScreenListener2 : FormListener {
    private readonly PC2Object _pcObjectOwner;
    public ShopScreenListener2(PC2Object owner, FormBase menu) : base(menu) => _pcObjectOwner = owner;
    [EventListener(PriorityLevels.HIGH)]
    private void OnMouseClick(MouseInputEvent ev, Vector2 pos) {
        UIManager.SubViewportClick(_pcObjectOwner.GetViewport(), _pcObjectOwner.GetCamera(), _pcObjectOwner.GetScreen(), ev);
    }
    [EventListener(PriorityLevels.HIGH)]
    private void OnMouseMove(MouseMoveEvent ev, Vector2 delta) {
        UIManager.SubViewportMouseMove(_pcObjectOwner.GetViewport(), _pcObjectOwner.GetCamera(), _pcObjectOwner.GetScreen(), ev);
        Vector2 uiPos = UIManager.GetSubViewportUIPos(_pcObjectOwner.GetViewport(), _pcObjectOwner.GetCamera(), _pcObjectOwner.GetScreen());
        _pcObjectOwner.GetShopMenu().GetForm().GetCursor()?.SetPosition(uiPos);
    }
}