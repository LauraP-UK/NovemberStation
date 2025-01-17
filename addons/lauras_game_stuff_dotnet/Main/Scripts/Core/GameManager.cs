
using System;
using System.Linq;
using Godot;

public class GameManager {
    
    private static readonly Scheduler scheduler = new();
    private static GameManager instance;

    private Node _activeScene, _sceneObjects;
    private Player _player;
    private Node _primaryMenu = null;
    
    private CanvasLayer _uiLayer;
    
    private GameManager() => instance = this;

    public static GameManager I() {
        if (instance == null) throw new InvalidOperationException("ERROR: GameManager.I() : GameManager instance is null. Did you forget to call GameManager.Init()?");
        return instance;
    }
    
    public static void Init() {
        if (instance != null) throw new InvalidOperationException("ERROR: GameManager.Init() : GameManager instance already exists.");
        new GameManager();
        
        EventManager eventManager = new();
        GameAction.Init();
    }
    
    public void SetActiveScene(Node scene) => _activeScene = scene;
    public void SetSceneObjects(Node sceneObjects) => _sceneObjects = sceneObjects;

    public Node GetActiveScene() {
        if (_activeScene == null) throw new InvalidOperationException("ERROR: GameManager.GetActiveScene() : Active scene is null. Set the active scene first.");
        return _activeScene;
    }
    
    public Node GetSceneObjects() {
        if (_sceneObjects == null) throw new InvalidOperationException("ERROR: GameManager.GetSceneObjects() : Scene objects are null. Set the scene objects first.");
        return _sceneObjects;
    }
    
    public void SetPlayer(Player player) => _player = player;
    public Player GetPlayer() {
        if (_player == null) throw new InvalidOperationException("ERROR: GameManager.GetPlayer() : Player is null. Set the player first.");
        return _player;
    }

    public void SetUILayer(CanvasLayer uiLayer = null) {
        if (uiLayer != null) {
            _uiLayer = uiLayer;
            return;
        }
        _uiLayer = GetActiveScene().GetTree().Root.GetNodeOrNull<CanvasLayer>("Main/UILayer");
    }

    public CanvasLayer GetUILayer() {
        if (_uiLayer == null) throw new InvalidOperationException("ERROR: GameManager.GetUILayer() : UI Layer is null. Set the UI Layer first.");
        return _uiLayer;
    }

    /* --- Game Methods --- */
    
    public void PopPauseMenu() {
        if (HasMenu("PauseMenu"))
            return;
        
        BinaryChoiceForm pauseMenu = new("PauseMenu");
        pauseMenu.SetTitle("Pause Menu");
        pauseMenu.SetDescription("Do you want to quit?");
        pauseMenu.SetUpperText("Resume");
        pauseMenu.SetLowerText("Quit");
        
        pauseMenu.SetBackgroundType(BinaryChoiceForm.BackgroundType.IMAGE);
        pauseMenu.SetBackgroundAlpha(0.5f);
        
        pauseMenu.OnUpperButton(_ => {
            pauseMenu.Destroy();
            CloseMenu("PauseMenu");
        });
        pauseMenu.OnLowerButton(_ => Quit());
        
        pauseMenu.SetKeyboardBehaviour((key, form, isPressed) => {
            if (!isPressed) return;
            switch (key) {
                case Key.W: {
                    form.GetUpperButton().GetElement().GrabFocus();
                    return;
                }
                case Key.S: {
                    form.GetLowerButton().GetElement().GrabFocus();
                    return;
                }
                case Key.Space: {
                    foreach (ButtonElement button in form.GetButtons().Where(button => button.GetElement().HasFocus())) {
                        button.ForcePressed();
                        return;
                    }
                    break;
                }
                case Key.Escape: {
                    form.GetUpperButton().ForcePressed();
                    return;
                }
            }
        });

        OpenMenu(pauseMenu, true);
    }

    public void Quit() => GetActiveScene().GetTree().Quit();
    public Rid GetWorldRid() => GetActiveScene().GetTree().Root.GetWorld3D().Space;
    public void Pause(bool pause) => GetActiveScene().GetTree().Paused = pause;
    
    public bool HasMenu(string menuName) => GetUILayer().GetChildren().Any(child => child.Name == menuName);
    public void CloseMenu(string menuName) {
        if (GetUILayer().GetChildren().FirstOrDefault(child => child.Name == menuName) is not Control menu) return;
        
        if (menu == _primaryMenu) _primaryMenu = null;
        
        GetUILayer().RemoveChild(menu);
        menu.QueueFree();
        Pause(false);
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetPlayer().GetController().SetLocked(false);
    }
    
    public void OpenMenu(FormBase menu, bool isPrimaryMenu = false) {
        if (HasMenu(menu.GetMenu().Name) || _primaryMenu != null) return;
        if (isPrimaryMenu) _primaryMenu = menu.GetMenu();
        GetUILayer().AddChild(menu.GetMenu());
        Pause(true);
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetPlayer().GetController().SetLocked(true);
    }
}