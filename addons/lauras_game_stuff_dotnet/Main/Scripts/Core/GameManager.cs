
using System;
using System.Linq;
using Godot;

public class GameManager {
    
    private static readonly Scheduler scheduler = new();
    private static GameManager instance;

    private Node _activeScene;
    private Player _player;
    
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

    public Node GetActiveScene() {
        if (_activeScene == null) throw new InvalidOperationException("ERROR: GameManager.GetActiveScene() : Active scene is null. Set the active scene first.");
        return _activeScene;
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
        
        if (_uiLayer.GetChildren().Any(child => child.Name == "PauseMenu"))
            return;
        
        Player player = GetPlayer();
        CanvasLayer uiLayer = GetUILayer();

        Input.MouseMode = Input.MouseModeEnum.Visible;
        player.GetController().SetLocked(true);
        
        /*TestDisplayForm testDisplayForm = new("TestMenu");
        ScrollDisplayList scrollDisplayList = testDisplayForm.GetScrollDisplay();
        scrollDisplayList.SetFollowFocus(true);
        
        scrollDisplayList.SetOnSelectElement<ButtonElement>(element => element.ForcePressed());
        testDisplayForm.SetOnReady(form => {
            for (int i = 0; i < 20; i++) {
                
                ColorRectElement colorRectElement = new(new ColorRect());
                colorRectElement.SetColor(
                    Randf.Random(0.0f, 1.0f), 
                    Randf.Random(0.0f, 1.0f),
                    Randf.Random(0.0f, 1.0f),
                    1.0f
                    );
                colorRectElement.SetSize(new Vector2(0, 150));
                colorRectElement.SetTopLevelLayout(form.GetTopLevelLayout());
                form.GetScrollDisplay().AddElement(colorRectElement);
                
                TextureRectElement textureRectElement = new("res://Main/Prefabs/UI/FormElements/TextureRectDefault.tscn");
                textureRectElement.SetTexture("res://Main/Textures/Placeholder/TestBG001.jpg");
                textureRectElement.SetSize(new Vector2(0, 150));
                textureRectElement.SetTopLevelLayout(testDisplayForm.GetTopLevelLayout());
                form.GetScrollDisplay().AddElement(textureRectElement);
                
                LabelElement labelElement = new("res://Main/Prefabs/UI/FormElements/LabelDefault.tscn", label => {
                    GD.Print($"Setting text for label {i}");
                    label.SetText("Label " + i);
                });
                labelElement.SetTopLevelLayout(form.GetTopLevelLayout());
                form.GetScrollDisplay().AddElement(labelElement);
                
                ButtonElement button = new("res://Main/Prefabs/UI/FormElements/ButtonDefault.tscn");
                button.GetElement().SetText("Button " + i);
                button.OnPressed(elem => {
                    Button btn = ((ButtonElement)elem).GetElement();
                    GD.Print("Button " + btn.Text + " pressed!");
                });
                button.SetTopLevelLayout(form.GetTopLevelLayout());
                form.GetScrollDisplay().AddElement(button);
            }
        });
        
        uiLayer.AddChild(testDisplayForm.GetMenu());

        Pause(true);
        return;*/
        BinaryChoiceForm pauseMenu = new("PauseMenu");
        
        pauseMenu.SetTitle("Pause Menu");
        pauseMenu.SetDescription("Do you want to quit?");
        pauseMenu.SetUpperText("Resume");
        pauseMenu.SetLowerText(Randf.RandomChanceIn(1, 10) ? "Quip?" : "Quit");
        
        pauseMenu.SetBackgroundType(BinaryChoiceForm.BackgroundType.IMAGE);
        pauseMenu.SetBackgroundAlpha(0.5f);
        
        pauseMenu.OnUpperButton(_ => {
            uiLayer.RemoveChild(pauseMenu.GetMenu());
            Input.MouseMode = Input.MouseModeEnum.Captured;
            player.GetController().SetLocked(false);
            pauseMenu.Destroy();
            Pause(false);
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

        Pause(true);
        uiLayer.AddChild(pauseMenu.GetMenu());
    }

    public void Quit() => GetActiveScene().GetTree().Quit();
    public Rid GetWorldRid() => GetActiveScene().GetTree().Root.GetWorld3D().Space;
    public void Pause(bool pause) => GetActiveScene().GetTree().Paused = pause;
}