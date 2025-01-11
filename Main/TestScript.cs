
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {

    private static TestScript instance;
    private static readonly Scheduler scheduler = new();
    
    private Player player;

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
        
        Input.MouseMode = Input.MouseModeEnum.Captured;
        
        player = (Player) Characters.PLAYER.CreateActor();
        GetTree().Root.GetNode<Node>("Main").AddChild(player.GetModel());
        player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));

        foreach (Node child in GetTree().Root.GetNode<Node3D>("Main/SceneObjects").GetChildren()) {
            if (child is RigidBody3D obj) {
                _dynamicObjects.Add(obj, obj.GlobalPosition);
                obj.AngularDamp = 0.5f;
            }
        }

        GD.Print($"Dynamic Objects: {_dynamicObjects.Count}");
        CallDeferred("TestMenu");
    }

    private void TestMenu() {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        player.GetController().SetLocked(true);
        
        CanvasLayer uiLayer = GetTree().Root.GetNodeOrNull<CanvasLayer>("UILayer");
        if (uiLayer == null) {
            uiLayer = new CanvasLayer();
            uiLayer.Name = "UILayer";
            GetTree().Root.AddChild(uiLayer);
        }
        
        TextureElement textureElement = new("res://Main/Prefabs/UI/FormElements/TextureRectDefault.tscn",
            rect => {
                rect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
                rect.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                rect.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
                rect.Size = new Vector2(400, 300);
            });
        ButtonElement buttonElement = new("res://Main/Prefabs/UI/FormElements/ButtonDefault.tscn",
            button => button.SetText("Click to play!"));
        ButtonElement buttonElement2 = new("res://Main/Prefabs/UI/FormElements/ButtonDefault.tscn",
            button => {
                button.SetText("Click to play!");
                button.Position = buttonElement.GetElement().Position + new Vector2(buttonElement.GetElement().Size.X, 0.0f);
            });
        
        int clickCount = 0;

        /*buttonElement.OnButtonDown((element, menu) => {
            ButtonElement button = (ButtonElement) element;
            Container menuContainer = (Container) menu;
            
            button.GetElement().SetText("AAHHH! Don't press me!");
        });
        buttonElement.OnToggled((element, menu, objParams) => {
            ButtonElement button = (ButtonElement) element;
            Container menuContainer = (Container) menu;
            bool toddledState = (bool) objParams[0];
            
            button.GetElement().SetText("I'm toggled: " + toddledState);
        });*/
        
        buttonElement.OnPressed(formObject => {
            Button element = ((ButtonElement)formObject).GetElement();

            switch (clickCount) {
                case 0: {
                    element.SetText("Jokes on you, there's no game!");
                    element.Position += new Vector2(0.0f, element.Size.Y);
                    break;
                }
                case 1: {
                    element.SetText("Ok, fine. Click again.");
                    element.Position += new Vector2(0.0f, element.Size.Y);
                    break;
                }
                case 2: {
                    player.GetController().SetLocked(false);
                    GetTree().Root.RemoveChild(uiLayer);
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                    break;
                }
            }
            clickCount++;
        });
        buttonElement2.OnPressed(formObject => {
            ButtonElement buttonElement = (ButtonElement)formObject;
            Button element = buttonElement.GetElement();
            string random = Randf.Random("Haha, I moved!", "Try again!", "Can't catch me!", "I'm over here now!", "Too slow!", "Neh neh neh neh neh!", "Kachow!", "Bazinga!");
            element.SetText(random);
            element.Position = new Vector2(Randf.Random(0.0f, 200.0f), Randf.Random(0.0f, 200.0f));
        });
        
        ContainerLayout layout1 = new(new Container());
        ContainerLayout layout2 = new(new Container());
        ContainerLayout mainLayout = new(new Container());

        layout1.AddElement(textureElement);
        layout2.AddElement(buttonElement);
        layout2.AddElement(buttonElement2);
        
        mainLayout.AddElement(layout1);
        mainLayout.AddElement(layout2);

        Container builtMenu = mainLayout.Build();

        uiLayer.AddChild(builtMenu);
        
        buttonElement2.GetElement().Position = buttonElement.GetElement().Position + new Vector2(buttonElement.GetElement().Size.X, 0.0f);

        builtMenu.AnchorLeft = 0.25f;
        builtMenu.AnchorRight = 0.75f;
        builtMenu.AnchorTop = 0.25f;
        builtMenu.AnchorBottom = 0.75f;

        builtMenu.SetSize(new Vector2(400, 300));
        builtMenu.Position = new Vector2(400, 200);
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        MovementActionTracker.Update();

        if (player.GetModel().Position.Y < -20) player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));
        
        foreach (RigidBody3D physicsObject in _dynamicObjects.Keys) {
            if (physicsObject.GlobalPosition.Y < -20) {
                physicsObject.GlobalPosition = _dynamicObjects[physicsObject];
                physicsObject.LinearVelocity = new Vector3(physicsObject.LinearVelocity.X, 1.0f, physicsObject.LinearVelocity.Z);
                //physicsObject.AngularVelocity = Vector3.Zero;
            }
        }

        RaycastResult raycastResult = Raycast.Trace(player, 3.0f);
        List<RaycastResult.HitBodyData> hitObjs = raycastResult.GetHitsSortedByDistance().Where(obj => obj.Body is RigidBody3D).ToList();

        if (raycastResult.HasHit())
            foreach (RaycastResult.HitBodyData hitObj in hitObjs)
                HighlightObject((RigidBody3D) hitObj.Body);
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
        player.GetController().Update((float) delta);
    }
    
    public void Quit() => GetTree().Quit();
    public Rid GetWorldRid() => GetTree().Root.GetWorld3D().Space;

    public void AddActionNode(ActionNode node) {
        GetTree().Root.GetNode<Node>("Main/Actions").AddChild(node);
    }
}