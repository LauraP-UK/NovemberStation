
using NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI;

public class ContextMenu : PreMadeMenu<ContextMenuForm> {
    protected override FormBase Build() {
        ContextMenuForm contextMenu = new(GetFormName());
        contextMenu.Hide();
        return contextMenu;
    }
    protected override string GetFormName() => "ContextMenu";
    protected override bool IsPrimary() => false;
}