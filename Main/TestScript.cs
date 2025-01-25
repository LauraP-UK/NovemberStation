using System.Collections.Generic;
using System.Linq;
using Godot;

namespace NovemberStation.Main;

public partial class TestScript : Node {
    private readonly Dictionary<RigidBody3D, Vector3> _dynamicObjects = new();

    public TestScript() {
        GameManager.Init();
        GameManager.I().SetActiveScene(this);
    }

    public override void _Input(InputEvent @event) => InputController.ProcessInput(@event);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        GD.Print("Start");
        
        EventManager.HookWindowResize(GetViewport());
        UIManager.SetUILayer();
        
        GameManager gameManager = GameManager.I();
        gameManager.SetSceneObjects(GetTree().Root.GetNode<Node3D>("Main/SceneObjects"));

        Input.MouseMode = Input.MouseModeEnum.Captured;

        Player player = (Player)Characters.PLAYER.CreateActor();
        GetTree().Root.GetNode<Node>("Main/PlayerHolder").AddChild(player.GetModel());
        player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));
        gameManager.SetPlayer(player);

        Node3D pcNode = GetTree().Root.GetNode<Node3D>("Main/PC");
        SubViewport subViewport = pcNode.GetNode<SubViewport>("Screen/ScreenViewport");
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
        shopMenu.DisplayOn(subViewport);

        foreach (Node child in gameManager.GetSceneObjects().GetChildren()) {
            if (child is not RigidBody3D obj) continue;
            _dynamicObjects.Add(obj, obj.GlobalPosition);
            obj.AngularDamp = 0.5f;
        }

        GD.Print($"Dynamic Objects: {_dynamicObjects.Count}");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        MovementActionTracker.Update();
        Player player = GameManager.I().GetPlayer();

        if (player.GetModel().Position.Y < -20) player.SetPosition(new Vector3(5f, 0.2f, 0f), new Vector3(0.0f, 90.0f, 0.0f));

        Node sceneObjects = GameManager.I().GetSceneObjects();
        
        foreach (Node child in sceneObjects.GetChildren()) {
            if (child is not RigidBody3D physicsObj) continue;
            if (!(physicsObj.GlobalPosition.Y < -20)) continue;
            if (_dynamicObjects.TryGetValue(physicsObj, out Vector3 origPosition))
                physicsObj.GlobalPosition = origPosition;
            else
                physicsObj.QueueFree();
        }

        RaycastResult raycastResult = Raycast.Trace(player, 3.0f);
        RaycastResult.HitBodyData firstHit = raycastResult.GetClosestHit();
        if (!raycastResult.HasHit() || firstHit.Body is not RigidBody3D rigidBody3D) {
            PlayerController playerController = player.GetController<PlayerController>();
            playerController.HideContextBox();
            return;
        }
        HighlightObject(rigidBody3D);
    }

    public override void _PhysicsProcess(double delta) {
        base._PhysicsProcess(delta);
        Player player = GameManager.I().GetPlayer();
        if (!GetTree().Paused) player.GetController().Update((float)delta);
    }

    private static void HighlightObject(RigidBody3D obj) {
        CollisionShape3D shape = obj.GetNode<CollisionShape3D>("BBox");

        BoundingBox bb = BoundingBox.FromCollisionMesh(shape);
        Vector2[] inScreenSpace = bb.GetCornersInScreenSpace(GameManager.I().GetPlayer().GetCamera(), obj.Transform);
        Vector2[] extremesVector2 = VectorUtils.GetExtremes(inScreenSpace);

        PlayerController playerController = GameManager.I().GetPlayer().GetController<PlayerController>();
        Vector2 minPos = extremesVector2[2];
        Vector2 maxPos = extremesVector2[0] - minPos;
        playerController.DrawContextBox(minPos, maxPos);
    }
}