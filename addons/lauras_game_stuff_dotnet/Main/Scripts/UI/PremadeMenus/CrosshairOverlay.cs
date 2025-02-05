
public class CrosshairOverlay : PreMadeMenu<CrosshairForm> {
    protected override FormBase Build() {
        CrosshairForm form = new(GetFormName());
        return form;
    }
    protected override string GetFormName() => "CrosshairOverlay";
    protected override bool IsPrimary() => false;
}