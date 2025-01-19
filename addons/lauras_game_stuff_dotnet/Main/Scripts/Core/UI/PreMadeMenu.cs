namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI;

public abstract class PreMadeMenu {

    public void Open(bool isPrimary = true) {
        if (UIManager.HasMenu(GetFormName())) return;
        UIManager.OpenMenu(Build(), isPrimary);
    }

    public void Close() {
        if (!UIManager.HasMenu(GetFormName())) return;
        UIManager.CloseMenu(GetFormName());
    }
    
    protected abstract FormBase Build();
    protected abstract string GetFormName();

}