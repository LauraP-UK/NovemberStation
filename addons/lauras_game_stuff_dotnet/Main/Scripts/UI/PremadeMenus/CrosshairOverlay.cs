
public class CrosshairOverlay : PreMadeMenu<CrosshairForm> {
    protected override FormBase Build() {
        CrosshairForm form = new(GetFormName());
        EventManager.UnregisterListeners(form);
        return form;
    }
    protected override string GetFormName() => "CrosshairOverlay";
    protected override bool IsPrimary() => false;
}