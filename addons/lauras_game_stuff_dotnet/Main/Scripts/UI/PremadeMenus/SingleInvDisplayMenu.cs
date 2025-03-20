using Godot;

public class SingleInvDisplayMenu : PreMadeMenu<SingleInventoryForm> {
    protected override FormBase Build() {
        SingleInventoryForm form = new(GetFormName(), (key, _, isPressed) => {
            if (!isPressed) return;
            if (key != Key.Escape) return;
            Close();
        });
        
        form.SetPrimaryInventory(GameManager.I().GetPlayer());
        form.SetListener(FormListener.Default(form));
        
        return form;
    }

    protected override bool IsPrimary() => true;
    protected override string GetFormName() => "single_inventory_display_menu";
}