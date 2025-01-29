using System;
using Godot;

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
    
    public T GetForm() {
        if (_form == null) {
            GD.PrintErr($"ERROR: PreMadeMenu.GetForm() : Form {GetType()} - {GetFormName()} does not exist. Did you forget to call Open()?");
            return null;
        }
        return _form;
    }
}