
public class ToastUI : PreMadeMenu<ToastForm> {
    protected override FormBase Build() {
        ToastForm form = new(GetFormName());
        return form;
    }
    protected override string GetFormName() => "ToastUIForm";
    protected override bool IsPrimary() => false;
}