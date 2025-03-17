using Godot;

public class SingleInvDisplayMenu : PreMadeMenu<SingleInventoryDisplayForm> {
    protected override FormBase Build() {
        SingleInventoryDisplayForm form = new(GetFormName(), (key, form, isPressed) => {
            if (!isPressed) return;
            if (key != Key.Escape) return;
            Close();
        });
        
        form.SetMainInv(GameManager.I().GetPlayer());
        form.SetListener(FormListener.Default(form));
        
        return form;
    }

    protected override bool IsPrimary() => true;
    protected override string GetFormName() => "single_inventory_display_menu";
}