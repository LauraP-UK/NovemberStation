using Godot;

public class PCObject : ObjectBase<Node3D> {
    private const string
        SCREEN_VIEWPORT_PATH = "ScreenStatic/Screen/ScreenViewport",
        SCREEN_PATH = "ScreenStatic/Screen",
        CAMERA_PATH = "CameraPos/Camera3D";

    private readonly SubViewport _viewport;
    private readonly MeshInstance3D _screen;
    private readonly Camera3D _camera;

    public PCObject(Node3D pcNode) : base(pcNode) {
        _viewport = FindNode<SubViewport>(SCREEN_VIEWPORT_PATH);
        _screen = FindNode<MeshInstance3D>(SCREEN_PATH);
        _camera = FindNode<Camera3D>(CAMERA_PATH);
        
        ShopMenu shopMenu = new();
        shopMenu.ModifyForm(form => {
            form.SetCaptureInput(false);
            ScrollDisplayList display = form.GetScrollDisplay();
            display.SetKeyboardEnabled(false);
            display.SetCaptureInput(false);
            EventManager.UnregisterListeners(display);
            EventManager.UnregisterListeners(form);
            display.GetDisplayObjects().ForEach(EventManager.UnregisterListeners);
        });
        shopMenu.DisplayOn(_viewport);
    }

    public void View() => _camera.SetCurrent(true);
    public void Release() => GameManager.I().GetPlayer().GetCamera().SetCurrent(true);

    public new static string GetObjectTag() => "pc_obj";
}