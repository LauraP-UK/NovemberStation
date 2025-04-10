using Godot;

public class PaperSheetMenu : PreMadeMenu<PaperSheetForm> {
    protected override FormBase Build() {
        PaperSheetForm paperSheetForm = new(GetFormName(), "",(key, _, isPressed) => {
            if (!isPressed) return;
            switch (key) {
                case Key.Escape:
                    Close();
                    return;
            }
        });
        return paperSheetForm;
    }
    protected override string GetFormName() => "PaperSheetForm";

    protected override bool IsPrimary() => true;
}