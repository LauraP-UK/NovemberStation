
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
        
        _menus[menu]?.RemoveFromScene();
        _menus.Remove(menu);
        GameManager.I().GetPlayer().GetController().SetLocked(false);
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GameManager.I().Pause(false);
    }
    
    public static void OpenMenu(FormBase menu, bool isPrimaryMenu = false) {
        if (HasMenu(menu.GetMenu().Name) || _primaryUIOpen != null) return;
        if (isPrimaryMenu) _primaryUIOpen = menu.GetMenu();
        menu.AddToScene(GetUILayer());
        _menus.Add(menu.GetMenu(), menu);
        GameManager.I().GetPlayer().GetController().SetLocked(true);
        GameManager.I().Pause(true);
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    
}