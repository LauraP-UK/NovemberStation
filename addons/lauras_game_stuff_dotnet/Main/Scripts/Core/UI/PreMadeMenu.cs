using System;
using Godot;

namespace NovemberStation.addons.lauras_game_stuff_dotnet.Main.Scripts.Core.UI;

public abstract class PreMadeMenu<T> where T : FormBase {

    private Action<T> _modify;
    private T _form;
    
    public void Open() {
        if (UIManager.HasMenu(GetFormName())) return;
        
        if (_form != null) {
            GD.PrintErr($"ERROR: PreMadeMenu.Open() : Form {GetType()} - {GetFormName()} already exists.");
            return;
        }
        
        _form = (T) Build();
        _modify?.Invoke(_form);
        UIManager.OpenMenu(_form, IsPrimary());
    }

    public void Close() {
        if (!UIManager.HasMenu(GetFormName())) return;
        UIManager.CloseMenu(GetFormName());
    }
    
    public void DisplayOn(SubViewport viewport) {
        if (_form != null) {
            GD.PrintErr($"ERROR: PreMadeMenu.DisplayOn() : Form {GetType()} - {GetFormName()} already exists.");
            return;
        }
        FormBase form = Build();
        _modify?.Invoke((T)form);
        viewport.AddChild(form.GetMenu());
    }
    
    protected abstract FormBase Build();
    protected abstract string GetFormName();
    protected virtual bool IsPrimary() => true;

    public void ModifyForm(Action<T> modify) => _modify = modify;
    
    public T GetForm() => _form;
}