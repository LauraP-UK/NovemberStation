
using Godot;

public abstract class FormListener {
    
    private FormBase _menu;
    protected FormListener(FormBase menu) => _menu = menu;
    
    public void Register() => EventManager.RegisterListeners(this);
    public void Unregister() => EventManager.UnregisterListeners(this);

    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnKeyPress(KeyPressEvent ev, Key key) {
        if (_menu.GetTopLevelLayout().CaptureInput()) ev.Capture();
        GD.Print($"{_menu.GetNode().Name} - Key Pressed: {key}");
        _menu.KeyboardBehaviour(key, true);
    }

    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnKeyRelease(KeyReleaseEvent ev, Key key) {
        if (_menu.GetTopLevelLayout().CaptureInput()) ev.Capture();
        GD.Print($"{_menu.GetNode().Name} - Key Released: {key}");
        _menu.KeyboardBehaviour(key, false);
    }

    [EventListener(PriorityLevels.HIGHEST)]
    protected void OnMouseEvent(MouseInputEvent ev, Vector2 pos) {
        if (_menu.GetTopLevelLayout().CaptureInput()) ev.Capture();
    }
    
    public static FormListener Default(FormBase menu) => new DefaultListener(menu);
    private class DefaultListener : FormListener {
        public DefaultListener(FormBase menu) : base(menu) {}
    }
}