using System;
using Godot;

public class PCObject : ObjectBase<Node3D>, IUsable {
    private const string
        SCREEN_VIEWPORT_PATH = "ScreenStatic/Screen/ScreenViewport",
        SCREEN_PATH = "ScreenStatic/Screen",
        CAMERA_PATH = "CameraPos/Camera3D";

    private readonly SubViewport _viewport;
    private readonly MeshInstance3D _screen;
    private readonly Camera3D _camera;
    
    private readonly GameManager _gameManager = GameManager.I();

    public PCObject(Node3D pcNode) : base(pcNode, "pc_obj", "pc_obj") {
        RegisterAction<IUsable>((_,_) => true, Use);

        string finding = "NULL";
        try {
            finding = SCREEN_VIEWPORT_PATH;
            _viewport = FindNode<SubViewport>(SCREEN_VIEWPORT_PATH);
            finding = SCREEN_PATH;
            _screen = FindNode<MeshInstance3D>(SCREEN_PATH);
            finding = CAMERA_PATH;
            _camera = FindNode<Camera3D>(CAMERA_PATH);
        }
        catch (Exception e) {
            GD.PrintErr($"WARN: PCObject.<init> : Failed to find required {finding} node.");
            return;
        }
        
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
    private void View() {
        _camera.SetCurrent(true);
        _gameManager.GetPlayer().GetController<PlayerController>().SetLocked(true);
        _gameManager.SetMouseControl(true);
    }

    private void Release() {
        _gameManager.GetPlayer().GetCamera().SetCurrent(true);
        _gameManager.GetPlayer().GetController<PlayerController>().SetLocked(false);
        _gameManager.SetMouseControl(false);
    }

    public void Use(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;
        if (_camera.IsCurrent()) Release();
        else View();
    }
}