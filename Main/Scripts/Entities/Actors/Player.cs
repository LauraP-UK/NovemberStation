using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Player : ActorBase, IViewable, IHotbarActor {
    private readonly Camera3D _camera;
    private readonly Node3D _crouchNode, _handNode, _handOrientation, _leanNode, _camContainer;
    
    private IObjectBase _handItem;
    private readonly Hotbar _hotbar;

    private readonly VolumetricInventory _inv;

    public Player(CharacterBody3D body) : base(body) {
        _leanNode = GetModel().GetNode<Node3D>("HeadOrigin/LeanControl");
        _camContainer = GetModel().GetNode<Node3D>("HeadOrigin/LeanControl/CrouchOrigin/CrouchControl/CamContainer");
        _crouchNode = GetModel().GetNode<Node3D>("HeadOrigin/LeanControl/CrouchOrigin/CrouchControl");
        _camera = GetModel().GetNode<Camera3D>("HeadOrigin/LeanControl/CrouchOrigin/CrouchControl/CamContainer/Camera");
        _handNode = GetModel().GetNode<Node3D>("HeadOrigin/LeanControl/CrouchOrigin/CrouchControl/CamContainer/Camera/HandPoint");
        _handOrientation = GetModel().GetNode<Node3D>("HeadOrigin/LeanControl/CrouchOrigin/CrouchControl/CamContainer/Camera/HandPoint/ObjOrientation");

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
        
        _hotbar = new Hotbar(this);
        new PlayerController(this);
    }

    public IObjectBase GetHandItem() => _handItem;
    public Camera3D GetCamera() => _camera;
    public void AssumeCameraControl() {
        Camera3D camera3D = GetCamera();
        camera3D.MakeCurrent();
        camera3D.SetPosition(Vector3.Zero);
    }
    public Node3D GetHandNode() => _handNode;
    public Node3D GetCamContainer() => _camContainer;
    public Node3D GetCrouchNode() => _crouchNode;
    public Node3D GetLeanNode() => _leanNode;
    public Node3D GetHandOrientation() => _handOrientation;
    public Hotbar GetHotbar() => _hotbar;

    public IObjectBase SetHeldItem(string json) {
        ClearHeldItem();
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
        
        GameManager.RegisterObject(item, objClass);
        
        return objClass;
    }
    public IObjectBase SetHeldItem(ItemType itemType) {
        _handItem?.GetBaseNode3D().QueueFree();

        if (itemType == null) {
            _handItem = null;
            HeldDisplaySettings.Default().ApplyTo(GetHandOrientation());
            return null;
        }
        
        RigidBody3D item = itemType.CreateInstance();
        IObjectBase objClass = GameManager.RegisterObject(item);
        
        GetHandOrientation().AddChild(item);
        itemType.ApplyHeldOrientation(GetHandOrientation());
        item.FreezeMode = RigidBody3D.FreezeModeEnum.Static;
        item.Freeze = true;
        itemType.SetCollision(false, item);
        _handItem = objClass;
        itemType.TryOnDataSpawn(objClass);
        
        GameManager.RegisterObject(item, objClass);
        
        return objClass;
    }

    public void ClearHeldItem() => SetHeldItem((ItemType)null);
    
    public RaycastResult GetLookingAt(float distance) => Raycast.Trace(this, distance);
    public ActorBase GetActor() => this;
    public float GetLookSmoothness() => 150.0f;

    public IInventory GetInventory() => _inv;

    public AddItemFailCause StoreItem(ItemType itemType) {
        RigidBody3D node = itemType.CreateInstance();
        IObjectBase obj = GameManager.RegisterObject(node);
        AddItemFailCause result = StoreItem(obj, node);
        node.QueueFree();
        return result;
    }

    public AddItemFailCause StoreItem(IObjectBase objectBase, Node node) {
        AddItemFailCause result = GetInventory().AddItem(objectBase);
        if (result != AddItemFailCause.SUCCESS) return result;
        if (!GetHotbar().IsFull()) {
            GetHotbar().AddToHotbar(objectBase.GetGUID());
            GetHotbar().UpdateOwnerHeldItem();
        }
        node.QueueFree();
        return result;
    }

    public AddItemFailCause StoreItem(string objectMetaTag, string objectJson) {
        AddItemFailCause result = GetInventory().AddItem(objectMetaTag, objectJson);
        if (result != AddItemFailCause.SUCCESS || GetHotbar().IsFull()) return result;
        string guidString = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, objectJson);
        Guid guid = Guid.Parse(guidString);
        GetHotbar().AddToHotbar(guid);
        GetHotbar().UpdateOwnerHeldItem();
        return result;
    }

    public bool RemoveItem(string objectJson) {
        VolumetricInventory inv = GetInventory().GetAs<VolumetricInventory>();
        string tag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.META_TAG, objectJson);
        inv.RemoveItem(tag, objectJson);
        string guidString = Serialiser.GetSpecificData<string>(IObjectBase.GUID_KEY, objectJson);
        GetHotbar().RemoveFromHotbar(Guid.Parse(guidString));
        return true;
    }
}