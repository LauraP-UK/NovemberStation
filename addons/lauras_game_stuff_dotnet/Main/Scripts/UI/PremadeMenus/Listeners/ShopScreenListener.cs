
using Godot;

public class ShopScreenListener : FormListener {
    private readonly PCObject _pcObjectOwner;
    public ShopScreenListener(PCObject owner, FormBase menu) : base(menu) => _pcObjectOwner = owner;
    [EventListener(PriorityLevels.HIGH)]
    private void OnMouseClick(MouseInputEvent ev, Vector2 pos) {
        UIManager.SubViewportClick(_pcObjectOwner.GetViewport(), _pcObjectOwner.GetCamera(), _pcObjectOwner.GetScreen(), ev);
    }
    [EventListener(PriorityLevels.HIGH)]
    private void OnMouseMove(MouseMoveEvent ev, Vector2 delta) {
        UIManager.SubViewportMouseMove(_pcObjectOwner.GetViewport(), _pcObjectOwner.GetCamera(), _pcObjectOwner.GetScreen(), ev);
    }
}