
using NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI;

public class CrosshairOverlay : PreMadeMenu<CrosshairForm> {
    protected override FormBase Build() {
        CrosshairForm form = new(GetFormName());
        return form;
    }
    protected override string GetFormName() => "CrosshairOverlay";
    protected override bool IsPrimary() => false;
}