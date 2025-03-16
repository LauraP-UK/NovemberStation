using System;
using Godot;

public abstract class PreMadeMenu<T> where T : FormBase {

    private Action<T> _modify;
    private T _form;
    
    public void Open() {
        if (UIManager.HasMenu(GetFormName())) return;
        
        _form = (T) Build();
        _modify?.Invoke(_form);
        
        UIManager.OpenMenu(_form, IsPrimary());
    }

    public void Close() {
        if (!UIManager.HasMenu(GetFormName())) {
            GD.PrintErr($"ERROR: PreMadeMenu.Close() : Form {GetType()} - {GetFormName()} does not exist.");
            return;
        }
        UIManager.CloseMenu(GetFormName());
    }
    
    public void DisplayOn(SubViewport viewport) {
        if (_form != null) {
            GD.PrintErr($"ERROR: PreMadeMenu.DisplayOn() : Form {GetType()} - {GetFormName()} already exists.");
            return;
        }
        _form = (T) Build();
        _modify?.Invoke(_form);
        
        if (AddCursor()) _form.SetDefaultCursor();
        
        viewport.AddChild(_form.GetMenu());
    }
    
    protected abstract FormBase Build();
    protected abstract string GetFormName();
    protected virtual bool IsPrimary() => true;
    protected virtual bool AddCursor() => false;

    public void ModifyForm(Action<T> modify) => _modify = modify;
    
    public T GetForm() {
        if (_form != null) return _form;
        GD.PrintErr($"ERROR: PreMadeMenu.GetForm() : Form {GetType()} - {GetFormName()} does not exist. Did you forget to call Open() or DisplayOn()?");
        return null;
    }
}