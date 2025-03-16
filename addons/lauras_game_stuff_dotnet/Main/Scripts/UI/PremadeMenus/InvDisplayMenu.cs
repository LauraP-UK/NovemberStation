

using Godot;

public class InvDisplayMenu : PreMadeMenu<InventoryDisplayForm> {
    protected override FormBase Build() {
        InventoryDisplayForm form = new(GetFormName(), (key, form, isPressed) => {
            if (!isPressed) return;
            if (key != Key.Escape) return;
            Close();
        });
        
        form.SetMainInv(GameManager.I().GetPlayer());
        form.SetListener(FormListener.Default(form));
        
        return form;
    }

    protected override bool IsPrimary() => true;
    protected override string GetFormName() => "inventory_display_menu";
}