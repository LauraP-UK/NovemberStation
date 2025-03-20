

using Godot;

public class DualInvDisplayMenu : PreMadeMenu<DualInventoryForm> {
    protected override FormBase Build() {
        DualInventoryForm form = new(GetFormName(), (key, _, isPressed) => {
            if (!isPressed) return;
            if (key != Key.Escape) return;
            Close();
        });
        
        form.SetPrimaryInventory(GameManager.I().GetPlayer());
        form.SetListener(FormListener.Default(form));
        
        return form;
    }

    protected override bool IsPrimary() => true;
    protected override string GetFormName() => "dual_inventory_display_menu";
}