using System.Collections.Generic;
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
        _uiLayer = GameManager.I().GetActiveScene().GetTree().Root.GetNodeOrNull<CanvasLayer>("Main/UILayer");
    }

    public static CanvasLayer GetUILayer() {
        if (_uiLayer == null) SetUILayer();
        return _uiLayer;
    }

    public static bool HasMenu(string menuName) => GetUILayer().GetChildren().Any(child => child.Name == menuName);

    public static void CloseMenu(string menuName) {
        if (GetUILayer().GetChildren().FirstOrDefault(child => child.Name == menuName) is not Control menu) return;

        if (menu == _primaryUIOpen) _primaryUIOpen = null;

        FormBase form = _menus[menu];
        _menus.Remove(menu);
        if (form != null && form.LockMovement()) {
            GameManager gameManager = GameManager.I();
            gameManager.GetPlayer().GetController().SetLocked(false);
            gameManager.Pause(false);
            gameManager.SetMouseControl(false);
        }

        form?.RemoveFromScene();
    }

    public static void OpenMenu(FormBase menu, bool isPrimaryMenu = false) {
        if (HasMenu(menu.GetMenu().Name) || _primaryUIOpen != null) return;
        if (isPrimaryMenu) _primaryUIOpen = menu.GetMenu();
        menu.AddToScene(GetUILayer());
        _menus.Add(menu.GetMenu(), menu);

        if (!menu.LockMovement()) return;
        GameManager gameManager = GameManager.I();
        gameManager.GetPlayer().GetController().SetLocked(true);
        gameManager.Pause(true);
        gameManager.SetMouseControl(true);
    }

    public static void Process(double delta) {
        foreach (KeyValuePair<Control, FormBase> pair in _menus) {
            FormBase form = pair.Value;
            if (form.RequiresProcess()) form.Process(delta);
        }
    }

    public static Vector2 GetSubViewportUIPos(SubViewport viewport, Camera3D camera3D, MeshInstance3D mesh) {
        Vector2 uv = GetUVFromHit(GetMouseHitCoords(camera3D), mesh);
        Vector2 size = viewport.GetVisibleRect().Size;
        return uv * size;
    }
    
    public static void SubViewportClick(SubViewport viewport, Camera3D camera3D, MeshInstance3D mesh, MouseInputEvent ev) {
        Vector2 uiPos = GetSubViewportUIPos(viewport, camera3D, mesh);

        InputEvent mouseButton = new InputEventMouseButton {
            Position = uiPos,
            GlobalPosition = uiPos,
            ButtonIndex = ev.GetMouseButton(),
            Pressed = ev.IsPressed()
        };
        
        viewport.PushInput(mouseButton);
    }
    
    public static void SubViewportMouseMove(SubViewport viewport, Camera3D camera3D, MeshInstance3D mesh, MouseMoveEvent ev) {
        Vector2 uiPos = GetSubViewportUIPos(viewport, camera3D, mesh);

        InputEvent mouseMotion = new InputEventMouseMotion {
            Position = uiPos,
            GlobalPosition = uiPos,
            Relative = ev.GetDelta()
        };
        
        viewport.PushInput(mouseMotion);
    }

    public static Vector3 GetMouseHitCoords(Camera3D camera) {
        Vector2 mousePos = GameManager.I().GetViewport().GetMousePosition();
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