using System.Collections.Generic;
using System.Linq;
using Godot;

public class Player : ActorBase, IViewable, IContainer {
    private readonly Camera3D _camera;
    private readonly Node3D _crouchNode, _handNode, _handOrientation;
    
    private IObjectBase _handItem;

    private readonly VolumetricInventory _inv;

    public Player(CharacterBody3D body) : base(body) {
        new PlayerController(this);
        _crouchNode = GetModel().GetNode<Node3D>("HeadOrigin/CrouchControl");
        _handNode = GetModel().GetNode<Node3D>("HeadOrigin/CrouchControl/Camera/HandPoint");
        _handOrientation = GetModel().GetNode<Node3D>("HeadOrigin/CrouchControl/Camera/HandPoint/ObjOrientation");
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

    public IObjectBase GetHandItem() => _handItem;
    public Camera3D GetCamera() => _camera;
    public Node3D GetCrouchNode() => _crouchNode;
    public Node3D GetHandOrientation() => _handOrientation;

    public IObjectBase SetHeldItem(string json) {
        ObjectAtlas.CreatedObject objInfo = ObjectAtlas.CreatedObjectFromJson(json);
        if (!objInfo.Success) {
            objInfo.Node?.QueueFree();
            return null;
        }

        RigidBody3D item = (RigidBody3D)objInfo.Node;
        IObjectBase objClass = objInfo.Object;
        
        string itemID = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, json);
        ItemType itemType = Items.GetViaID(itemID);

        GetHandOrientation().AddChild(item);
        itemType.ApplyHeldOrientation(GetHandOrientation());
        item.FreezeMode = RigidBody3D.FreezeModeEnum.Static;
        item.Freeze = true;
        itemType.SetCollision(false, item);
        _handItem = objClass;
        
        GameManager.I().RegisterObject(item, objClass);
        
        return objClass;
    }
    public IObjectBase SetHeldItem(ItemType itemType) {
        _handItem?.GetBaseNode3D().QueueFree();
        
        RigidBody3D item = itemType.CreateInstance();
        IObjectBase objClass = GameManager.I().RegisterObject(item);
        
        GetHandOrientation().AddChild(item);
        itemType.ApplyHeldOrientation(GetHandOrientation());
        item.FreezeMode = RigidBody3D.FreezeModeEnum.Static;
        item.Freeze = true;
        itemType.SetCollision(false, item);
        _handItem = objClass;
        itemType.TryOnDataSpawn(objClass);
        
        GameManager.I().RegisterObject(item, objClass);
        
        return objClass;
    }

    public RaycastResult GetLookingAt(float distance) => Raycast.Trace(this, distance);
    public ActorBase GetActor() => this;

    public IInventory GetInventory() => _inv;

    public AddItemFailCause StoreItem(ItemType itemType) {
        RigidBody3D node = itemType.CreateInstance();
        IObjectBase obj = GameManager.I().RegisterObject(node);
        AddItemFailCause result = StoreItem(obj, node);
        node.QueueFree();
        return result;
    }

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
        string tag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.META_TAG, objectJson);
        inv.RemoveItem(tag, objectJson);
        return true;
    }
}