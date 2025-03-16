using System.Linq;
using Godot;

public class CubeObject : ObjectBase<RigidBody3D>, IGrabbable, IShovable, IContainer {
    private readonly VolumetricInventory _inventory;

    public CubeObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "cube_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_, _) => true, Grab);
        RegisterAction<IShovable>((_, _) => true, Shove);
        RegisterArbitraryAction("Extract Item", 5, (_, _) => !GetInventory().IsEmpty(), (_, ev) => {
            if (ev is not KeyPressEvent) return;
            VolumetricInventory inv = GetInventory().GetAs<VolumetricInventory>();
            string firstJson = inv.GetContents().First();
            RemoveItemIntoWorld(firstJson);
        });
        RegisterArbitraryAction("Open", 20, (_, _) => true, (actor, ev) => {
            if (ev is not KeyPressEvent) return;
            InvDisplayMenu invDisplayMenu = new();
            invDisplayMenu.ModifyForm(form => { form.SetOtherInv(this); });
            invDisplayMenu.Open();
        });

        _inventory = new VolumetricInventory(350.0f, this);
    }

    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Shove(ActorBase actorBase, IEventBase ev) => ShoveActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => Items.STORAGE_CRATE.GetItemName();
    public override string GetContext() => $"Contains: {_inventory.GetUsedSize()}/{_inventory.GetMaxSize()} kg";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
    public IInventory GetInventory() => _inventory;
    public string GetName() => GetDisplayName();

    public bool StoreItem(IObjectBase objectBase, Node node) {
        bool added = GetInventory().GetAs<VolumetricInventory>().AddItem(objectBase);
        if (added) node.QueueFree();
        return added;
    }

    public bool StoreItem(string objectMetaTag, string objectJson) => GetInventory().AddItem(objectMetaTag, objectJson);

    public bool RemoveItem(string objectJson) {
        VolumetricInventory inv = GetInventory().GetAs<VolumetricInventory>();
        IObjectBase obj = ObjectAtlas.DeserialiseDataWithoutNode(objectJson);
        inv.RemoveItem(obj.GetObjectTag(), objectJson);
        return true;
    }

    public bool RemoveItemIntoWorld(string objectJson) {
        ObjectAtlas.CreatedObject obj = ObjectAtlas.CreatedObjectFromJson(objectJson);
        if (!obj.Success) {
            obj.Node?.QueueFree();
            return false;
        }

        Vector3 spawn = GetBaseNode().GlobalPosition + new Vector3(0, 1, 0);
        Node3D objNode = (Node3D)obj.Node;
        GameManager.I().GetSceneObjects().AddChild(obj.Node);
        objNode.SetGlobalPosition(spawn);
        GameManager.I().RegisterObject(objNode, obj.Object);
        RemoveItem(objectJson);
        return true;
    }
}