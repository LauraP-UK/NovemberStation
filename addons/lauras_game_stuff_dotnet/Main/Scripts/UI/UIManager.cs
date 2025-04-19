using System.Linq;
using Godot;

public static class UIManager {
    private static Control _primaryUIOpen;
    private static CanvasLayer _uiLayer;

    private static readonly SmartDictionary<Control, FormBase> _menus = new();

    public static void SetUILayer(CanvasLayer uiLayer = null) {
        if (uiLayer != null) {
            _uiLayer = uiLayer;
            return;
        }
        _uiLayer = MainLauncher.FindNode<CanvasLayer>("Main/UILayer");
        GameManager.SetCanvasLayerFocus(false);
    }

    public static CanvasLayer GetUILayer() {
        if (_uiLayer == null) SetUILayer();
        return _uiLayer;
    }

    public static bool HasMenu(string menuName) => GetUILayer().GetChildren().Any(child => child.Name.ToString() == menuName);
    public static bool IsPrimaryMenuOpen() => _primaryUIOpen != null;

    public static void CloseMenu(string menuName) {
        if (GetUILayer().GetChildren().FirstOrDefault(child => child.Name.ToString() == menuName) is not Control menu) {
            GD.PrintErr($"ERROR: UIManager.CloseMenu() : Menu {menuName} does not exist.");
            return;
        }

        Player player = GameManager.GetPlayer();
        PlayerController controller = player.GetController<PlayerController>();

        if (menu == _primaryUIOpen) {
            _primaryUIOpen = null;
            GameManager.SetCanvasLayerFocus(false);
            controller.ShowUI(true);
        }

        FormBase form = _menus[menu];
        _menus.Remove(menu);
        if (form != null && form.LockMovement()) {
            controller.SetLocked(false);
            controller.ShowUI(true);
            GameManager.SetMouseControl(false);
        }

        if (form != null && form.PausesGame()) GameManager.Pause(false);

        form?.RemoveFromScene();
    }

    public static void OpenMenu(FormBase menu, bool isPrimaryMenu = false) {
        if (HasMenu(menu.GetMenu().Name.ToString()) || _primaryUIOpen != null) return;
        
        if (isPrimaryMenu) {
            _primaryUIOpen = menu.GetMenu();
            GameManager.SetCanvasLayerFocus(true);
            GameManager.GetPlayer().GetController<PlayerController>().ShowUI(false);
        }
        menu.AddToScene(GetUILayer());
        _menus.Add(menu.GetMenu(), menu);

        if (menu.LockMovement()) {
            PlayerController controller = GameManager.GetPlayer().GetController<PlayerController>();
            controller.SetLocked(true);
            controller.ShowUI(false);
            GameManager.SetMouseControl(true);
        }

        if (menu.PausesGame()) GameManager.Pause(true);
    }

    public static void Process(double delta) {
        foreach ((Control _, FormBase form) in _menus)
            if (form is IProcess formProcess)
                formProcess.Process((float)delta);
    }

    public static Vector2 GetSubViewportUIPos(SubViewport viewport, Camera3D camera3D, MeshInstance3D mesh) {
        Vector2 uv = GetUVFromHit(GetMouseHitCoords(camera3D), mesh);
        Vector2 size = viewport.GetVisibleRect().Size;
        return uv * size;
    }
    
    public static void SubViewportClick(SubViewport viewport, Camera3D camera3D, MeshInstance3D mesh, MouseInputEvent ev) {
        Vector2 uiPos = GetSubViewportUIPos(viewport, camera3D, mesh);

        InputEventMouseButton mouseButton = new(){
            Position = uiPos,
            GlobalPosition = uiPos,
            ButtonIndex =  ev.GetMouseButton(),
            Pressed = ev.IsPressed(),
            DoubleClick = false,
            Canceled = false
        };
        
        viewport.PushInput(mouseButton);
    }
    
    public static void SubViewportMouseMove(SubViewport viewport, Camera3D camera3D, MeshInstance3D mesh, MouseMoveEvent ev) {
        Vector2 uiPos = GetSubViewportUIPos(viewport, camera3D, mesh);

        InputEventMouseMotion mouseMotion = new(){
            Position = uiPos,
            GlobalPosition = uiPos,
            Relative = ev.GetDelta()
        };
        
        viewport.PushInput(mouseMotion);
    }

    public static Vector3 GetMouseHitCoords(Camera3D camera) {
        Vector2 mousePos = GameManager.GetMasterViewport().GetMousePosition();
        Vector3 from = camera.GetGlobalPosition();
        Vector3 to = camera.ProjectPosition(mousePos, 1f);
    
        RaycastResult result = Raycast.Trace(from, to);
        return !result.HasHit() ? new Vector3(float.NaN, float.NaN, float.NaN) : result.GetClosestHit().HitAtPosition;
    }

    public static Vector2 GetUVFromHit(Vector3 hitPosition, MeshInstance3D screen) {
        if (float.IsNaN(hitPosition.X)) return new Vector2(-1,-1); // Indicate invalid input
        Vector3 localHit = screen.ToLocal(hitPosition);
        Aabb localBounds = screen.GetAabb();
        Vector2 uv = new(
            Mathsf.InverseLerpClamped(localBounds.Position.X, localBounds.End.X, localHit.X),
            Mathsf.InverseLerpClamped(localBounds.Position.Z, localBounds.End.Z, localHit.Z)
        );
        return uv;
    }
}