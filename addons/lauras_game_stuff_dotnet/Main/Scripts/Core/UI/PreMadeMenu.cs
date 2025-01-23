using System;
using Godot;

namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI;

public abstract class PreMadeMenu<T> where T : FormBase {

    private Action<T> _modify;
    
    public void Open(bool isPrimary = true) {
        if (UIManager.HasMenu(GetFormName())) return;
        FormBase form = Build();
        _modify?.Invoke((T)form);
        UIManager.OpenMenu(form, isPrimary);
    }

    public void Close() {
        if (!UIManager.HasMenu(GetFormName())) return;
        UIManager.CloseMenu(GetFormName());
    }
    
    public void SendToViewport(SubViewport viewport) {
        FormBase form = Build();
        _modify?.Invoke((T)form);
        viewport.AddChild(form.GetMenu());
    }
    
    protected abstract FormBase Build();
    protected abstract string GetFormName();

    public void ModifyForm(Action<T> modify) => _modify = modify;
}