public class HotbarMenu : PreMadeMenu<HotbarForm> {
    protected override FormBase Build() {
        HotbarForm contextMenu = new(GetFormName());
        return contextMenu;
    }
    protected override string GetFormName() => "HotbarMenu";
    protected override bool IsPrimary() => false;
}