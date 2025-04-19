using System;
using Godot;

public class PCObject : ObjectBase<Node3D>, IUsable {
    private const string
        SCREEN_VIEWPORT_PATH = "ScreenStatic/Screen/ScreenViewport",
        SCREEN_PATH = "ScreenStatic/Screen",
        CAMERA_PATH = "ScreenStatic/CamPos/Camera3D",
        SPAWN_POINT_PATH = "Body/SpawnPoint",
        BODY_PATH = "ScreenStatic",
        LIGHT_PATH = "Tower/Light",
        TOWER_PATH = "Tower";

    private readonly SubViewport _viewport;
    private readonly MeshInstance3D _screen;
    private readonly Camera3D _camera;
    private readonly Node3D _spawnPoint;
    private readonly StaticBody3D _body, _tower;
    private readonly OmniLight3D _light;
    
    private readonly ShopMenu _shopMenu;
    
    public const string IS_ON_KEY = "isOn";
    [SerialiseData(IS_ON_KEY, nameof(Toggle), nameof(Toggle))]
    private bool _isOn;

    public PCObject(Node3D pcNode, bool dataOnly = false) : base(pcNode, "pc_obj") {
        if (dataOnly) return;

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
            finding = BODY_PATH;
            _body = FindNode<StaticBody3D>(BODY_PATH);
            finding = LIGHT_PATH;
            _light = FindNode<OmniLight3D>(LIGHT_PATH);
            finding = TOWER_PATH;
            _tower = FindNode<StaticBody3D>(TOWER_PATH);
        }
        catch (Exception e) {
            GD.PrintErr($"WARN: PC2Object.<init> : Failed to find required {finding} node.");
            return;
        }
        
        Toggle();
        
        AddInteractionZone(InteractionZoneBuilder<StaticBody3D, PCObject>.Builder("Screen", _body, this)
            .WithDisplayName("PC")
            .WithIsActive(() => _isOn)
            .WithAction<IUsable>((_,_) => true, Use)
            .Build());
        AddInteractionZone(InteractionZoneBuilder<StaticBody3D, PCObject>.Builder("Tower", _tower, this)
            .WithDisplayName("PC Tower")
            .WithArbitraryAction("Power On", 0, (_,_) => !_isOn, (_, ev) => {
                if (ev is not KeyPressEvent) return;
                Toggle(true);
            })
            .WithArbitraryAction("Power Off", 0, (_,_) => _isOn, (_, ev) => {
                if (ev is not KeyPressEvent) return;
                Toggle();
            })
            .Build());
        
        _shopMenu = new ShopMenu();
        _shopMenu.ModifyForm(form => {
            form.SetCaptureInput(false);
            form.GetScrollDisplay().SetFollowFocus(true);
            
            form.SetOnReady(form => {
                Items.GetItemButtons().ForEach(btn => {
                    btn.OnPressed(btn => {
                        ItemType itemType = btn.GetItemType();
                        RigidBody3D rigidBody3D = itemType.CreateInstance();
                        GameManager.GetSceneObjects().AddChild(rigidBody3D);
                        rigidBody3D.SetPosition(_spawnPoint.GlobalPosition + new Vector3(0,0.5f,0));

                        IObjectBase objData = GameManager.RegisterObject(rigidBody3D);
                        itemType.TryOnDataSpawn(objData);
                        
                        Toast.Info(GameManager.GetPlayer(), $"Purchased {itemType.GetItemName()} for {itemType.GetItemCost()}{ShopItemDisplayButton.CREDITS_SYMBOL}");
                    });

                    btn.SetTopLevelLayout(form.GetTopLevelLayout());
                    form.GetScrollDisplay().AddElement(btn);

                    Vector2 size = btn.GetNode().Size;
                    Vector2 curSize = btn.GetDescLabel().GetElement().GetSize();
                    btn.GetDescLabel().GetElement().SetSize(new Vector2(size.X * 0.7f, curSize.Y));
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
                            ((ShopItemDisplayButton) focusedElement)?.VisualPress(false);
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
    public ShopMenu GetShopMenu() => _shopMenu;

    public void Toggle(bool mode = false) {
        _isOn = mode;
        _light.SetVisible(_isOn);
        _screen.SetVisible(_isOn);
    }

    private void View() {
        _camera.SetCurrent(true);
        PlayerController playerController = GameManager.GetPlayer().GetController<PlayerController>();
        playerController.SetLocked(true);
        playerController.ShowUI(false);
        GameManager.SetMouseControl(true);
        GameManager.SetMouseVisible(false);
        TestDisplayForm form = _shopMenu.GetForm();
        ScrollDisplayList display = form.GetScrollDisplay();
        display.SetKeyboardEnabled(true);
        display.SetCaptureInput(true);
        display.GetListener().Register();
        GameManager.SyncBackdropCamera(_camera.GlobalRotation);
    }

    private void Release() {
        GameManager.GetPlayer().AssumeCameraControl();
        PlayerController playerController = GameManager.GetPlayer().GetController<PlayerController>();
        playerController.SetLocked(false);
        playerController.ShowUI(true);
        GameManager.SetMouseControl(false);
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

    public override string GetDisplayName() => "";
    public override string GetContext() => "";
    public override string GetSummary() => "";
}