using Godot;

public interface IFocusable {
    public void GrabFocus();
    public void ReleaseFocus();
    public bool HasFocus();
    public Control GetFocusableElement();
}