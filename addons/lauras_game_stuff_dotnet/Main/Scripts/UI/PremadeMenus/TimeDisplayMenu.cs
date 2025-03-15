public class TimeDisplayMenu : PreMadeMenu<TimeDisplayForm> {
    protected override FormBase Build() => new TimeDisplayForm();
    protected override string GetFormName() => "TimeDisplayMenu";
    protected override bool IsPrimary() => false;
}