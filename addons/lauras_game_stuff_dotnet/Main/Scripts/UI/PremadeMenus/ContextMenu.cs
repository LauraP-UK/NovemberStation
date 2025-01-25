
public class ContextMenu : PreMadeMenu<ContextMenuForm> {
    protected override FormBase Build() {
        ContextMenuForm contextMenu = new(GetFormName());
        contextMenu.Hide();
        return contextMenu;
    }
    protected override string GetFormName() => "ContextMenu";
    protected override bool IsPrimary() => false;
}