using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class PCObject : ObjectBase<Node3D>, IUsable {
    private const string
        SCREEN_VIEWPORT_PATH = "ScreenStatic/Screen/ScreenViewport",
        SCREEN_PATH = "ScreenStatic/Screen",
        CAMERA_PATH = "ScreenStatic/Screen/CameraPos/Camera3D",
        SPAWN_POINT_PATH = "Body/SpawnPoint";

    private readonly SubViewport _viewport;
    private readonly MeshInstance3D _screen;
    private readonly Camera3D _camera;
    private readonly Node3D _spawnPoint;
    
    private readonly GameManager _gameManager = GameManager.I();
    private readonly ShopMenu _shopMenu;

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
            finding = SPAWN_POINT_PATH;
            _spawnPoint = FindNode<Node3D>(SPAWN_POINT_PATH);
        }
        catch (Exception e) {
            GD.PrintErr($"WARN: PCObject.<init> : Failed to find required {finding} node.");
            return;
        }
        
        _shopMenu = new();
        _shopMenu.ModifyForm(form => {
            form.SetCaptureInput(false);
            
            form.SetOnReady(form => {
                Items.GetItemButtons().ForEach(btn => {
                    GameManager gameManager = GameManager.I();
                    btn.OnPressed(btn => {
                        ItemType itemType = btn.GetItemType();
                        RigidBody3D rigidBody3D = itemType.CreateInstance();
                        gameManager.GetSceneObjects().AddChild(rigidBody3D);
                        rigidBody3D.SetPosition(_spawnPoint.GlobalPosition);
                    
                        gameManager.RegisterObject(rigidBody3D);
                    
                        _shopMenu.Close();
                    });
                    btn.SetTopLevelLayout(form.GetTopLevelLayout());
                    form.GetScrollDisplay().AddElement(btn);
                });
            });
            
            ScrollDisplayList display = form.GetScrollDisplay();
            display.SetKeyboardBehaviour((pressedKey, form, isPressed) => {
                switch (pressedKey) {
                    case Key.W: {
                        if (!isPressed) return;
                        form.MoveFocus(-1);
                        break;
                    }
                    case Key.S: {
                        if (!isPressed) return;
                        form.MoveFocus(1);
                        break;
                    }
                    case Key.E:
                    case Key.Space: {
                        if (isPressed) {
                            if (form.GetOnSelectElement() == null) {
                                GD.PrintErr("ERROR: ScrollDisplayList.DefaultKeyboardBehaviour() : No onSelectElement action set.");
                                break;
                            }
                        
                            IFormObject focusedElement = form.GetFocusedElement() ?? form.FocusElement(0);
                            if (focusedElement != null) {
                                form.GetOnSelectElement().Invoke(focusedElement);
                                ((ShopItemDisplayButton) focusedElement).VisualPress(true);
                            }
                        }
                        else {
                            IFormObject focusedElement = form.GetFocusedElement();
                            ((ShopItemDisplayButton)focusedElement)?.VisualPress(false);
                        }
                        break;
                    }
                    case Key.Escape: {
                        Release();
                        break;
                    }
                }
            });
            display.SetKeyboardEnabled(false);
            display.SetCaptureInput(false);
            display.SetListener(new ShopScreenListener(this, display));
            display.SetRegisterListenerOnReady(false);
        });
        _shopMenu.DisplayOn(_viewport);
    }
    
    
    public SubViewport GetViewport() => _viewport;
    public MeshInstance3D GetScreen() => _screen;
    public Camera3D GetCamera() => _camera;
    
    
    private void View() {
        _camera.SetCurrent(true);
        _gameManager.GetPlayer().GetController<PlayerController>().SetLocked(true);
        _gameManager.SetMouseControl(true);
        TestDisplayForm form = _shopMenu.GetForm();
        ScrollDisplayList display = form.GetScrollDisplay();
        display.SetKeyboardEnabled(true);
        display.SetCaptureInput(true);
        display.GetListener().Register();
    }

    private void Release() {
        _gameManager.GetPlayer().GetCamera().SetCurrent(true);
        _gameManager.GetPlayer().GetController<PlayerController>().SetLocked(false);
        _gameManager.SetMouseControl(false);
        TestDisplayForm form = _shopMenu.GetForm();
        ScrollDisplayList display = form.GetScrollDisplay();
        display.SetKeyboardEnabled(false);
        display.SetCaptureInput(false);
        display.GetListener().Unregister();
    }

    public void Use(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;
        if (_camera.IsCurrent()) Release();
        else View();
    }
}