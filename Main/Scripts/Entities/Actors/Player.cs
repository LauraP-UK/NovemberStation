using System.Collections.Generic;
using System.Linq;
using Godot;

public class Player : ActorBase, IViewable, IContainer {
    private readonly Camera3D _camera;
    private readonly Node3D _crouchNode;

    private readonly VolumetricInventory _inv;

    public Player(CharacterBody3D body) : base(body) {
        new PlayerController(this);
        _crouchNode = GetModel().GetNode<Node3D>("HeadOrigin/CrouchControl");
        _camera = _crouchNode.GetNode<Camera3D>("Camera");

        List<VisualInstance3D> children = GetVisualModel()
            .GetChildren()
            .OfType<VisualInstance3D>()
            .ToList();

        foreach (VisualInstance3D child in children) {
            child.SetLayerMaskValue(1, false);
            child.SetLayerMaskValue(2, true);
        }

        GetCamera().SetCullMaskValue(2, false);
        _inv = new VolumetricInventory(200, this);
    }

    public Camera3D GetCamera() => _camera;
    public Node3D GetCrouchNode() => _crouchNode;

    public RaycastResult GetLookingAt(float distance) => Raycast.Trace(this, distance);
    public ActorBase GetActor() => this;

    public IInventory GetInventory() => _inv;

    public AddItemFailCause StoreItem(IObjectBase objectBase, Node node) {
        AddItemFailCause result = GetInventory().AddItem(objectBase);
        if (result == AddItemFailCause.SUCCESS) node.QueueFree();
        return result;
    }

    public AddItemFailCause StoreItem(string objectMetaTag, string objectJson) => GetInventory().AddItem(objectMetaTag, objectJson);

    public bool DropItem(string objectJson) {
        ObjectAtlas.CreatedObject obj = ObjectAtlas.CreatedObjectFromJson(objectJson);
        if (!obj.Success) return false;

        Vector3 spawn = GetLookingAt(2).GetEnd();
        Node3D objNode = (Node3D)obj.Node;
        GameManager.I().GetSceneObjects().AddChild(obj.Node);
        objNode.SetGlobalPosition(spawn);
        GameManager.I().RegisterObject(objNode, obj.Object);
        
        return RemoveItem(objectJson);
    }

    public bool RemoveItem(string objectJson) {
        VolumetricInventory inv = GetInventory().GetAs<VolumetricInventory>();
        string tag = Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.META_TAG, objectJson);
        inv.RemoveItem(tag, objectJson);
        return true;
    }
}