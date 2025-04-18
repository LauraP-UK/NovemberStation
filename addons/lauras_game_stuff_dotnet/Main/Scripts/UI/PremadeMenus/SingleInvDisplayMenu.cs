using Godot;

public class SingleInvDisplayMenu : PreMadeMenu<SingleInventoryForm> {
    protected override FormBase Build() {
        SingleInventoryForm form = new(GetFormName(), (key, _, isPressed) => {
            if (!isPressed) return;
            switch (key) {
                case Key.Escape:
                case Key.Tab:
                    Close();
                    return;
            }
        });
        
        form.SetPrimaryInventory(GameManager.GetPlayer());
        form.SetListener(FormListener.Default(form));
        
        return form;
    }

    protected override bool IsPrimary() => true;
    protected override string GetFormName() => "single_inventory_display_menu";
}