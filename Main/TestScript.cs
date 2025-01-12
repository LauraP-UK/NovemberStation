using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {
    private static TestScript instance;
    private static readonly Scheduler scheduler = new();

    private Player player;
    private CanvasLayer _uiLayer;

    private readonly Dictionary<RigidBody3D, Vector3> _dynamicObjects = new();

    public TestScript() {
        instance = this;
        EventManager eventManager = new();
        GameAction.Init();
    }

    public static TestScript I() => instance;

    public Player GetPlayer() => player;

    public override void _Input(InputEvent @event) => InputController.ProcessInput(@event);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        GD.Print("Start");
        
        EventManager.HookWindowResize(GetViewport());

        Input.MouseMode = Input.MouseModeEnum.Captured;

        player = (Player)Characters.PLAYER.CreateActor();
        GetTree().Root.GetNode<Node>("Main/PlayerHolder").AddChild(player.GetModel());
        player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));

        foreach (Node child in GetTree().Root.GetNode<Node3D>("Main/SceneObjects").GetChildren()) {
            if (child is RigidBody3D obj) {
                _dynamicObjects.Add(obj, obj.GlobalPosition);
                obj.AngularDamp = 0.5f;
            }
        }

        GD.Print($"Dynamic Objects: {_dynamicObjects.Count}");
        _uiLayer = GetTree().Root.GetNodeOrNull<CanvasLayer>("Main/UILayer");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        MovementActionTracker.Update();

        if (player.GetModel().Position.Y < -20) player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));

        foreach (RigidBody3D physicsObject in _dynamicObjects.Keys) {
            if (physicsObject.GlobalPosition.Y < -20) {
                physicsObject.GlobalPosition = _dynamicObjects[physicsObject];
                //physicsObject.LinearVelocity = Vector3.Zero;
                //physicsObject.AngularVelocity = Vector3.Zero;
            }
        }

        RaycastResult raycastResult = Raycast.Trace(player, 3.0f);
        List<RaycastResult.HitBodyData> hitObjs = raycastResult.GetHitsSortedByDistance().Where(obj => obj.Body is RigidBody3D).ToList();

        if (raycastResult.HasHit())
            foreach (RaycastResult.HitBodyData hitObj in hitObjs)
                HighlightObject((RigidBody3D)hitObj.Body);
    }

    private void HighlightObject(RigidBody3D obj) {
        foreach (Direction direction in Directions.GetAdjacent()) {
            Vector3 globalDirection = obj.GlobalTransform.Basis * direction.Offset;
            Vector3 endPoint = obj.GlobalPosition + globalDirection;

            DebugDraw.Line(obj.GlobalPosition, endPoint, direction.GetAxisDebugColor());
        }
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        if (!GetTree().Paused) player.GetController().Update((float)delta);
    }
    
    public void PopPauseMenu() {
        if (_uiLayer.GetChildren().Any(child => child.Name == "PauseMenu"))
            return;
        
        Input.MouseMode = Input.MouseModeEnum.Visible;
        player.GetController().SetLocked(true);
        
        BinaryChoiceForm pauseMenu = new("PauseMenu");
        pauseMenu.SetTitle("Pause Menu");
        pauseMenu.SetDescription("Do you want to quit?");
        pauseMenu.SetUpperText("Resume");
        pauseMenu.SetLowerText(Randf.RandomChanceIn(1, 10) ? "Quip?" : "Quit");
        pauseMenu.OnUpperButton(_ => {
            _uiLayer.RemoveChild(pauseMenu.GetMenu());
            Input.MouseMode = Input.MouseModeEnum.Captured;
            player.GetController().SetLocked(false);
            pauseMenu.Destroy();
            Pause(false);
        });
        pauseMenu.OnLowerButton(_ => Quit());
        pauseMenu.SetKeyboardBehaviour((key, form) => {
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
        _uiLayer.AddChild(pauseMenu.GetMenu());
    }

    public void Quit() => GetTree().Quit();
    public Rid GetWorldRid() => GetTree().Root.GetWorld3D().Space;
    public void Pause(bool pause) => GetTree().Paused = pause;
}