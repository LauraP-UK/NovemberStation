
using System.Linq;
using Godot;

public class CubeObject : ObjectBase<RigidBody3D>, IGrabbable, IShovable, IContainer {
    private readonly VolumetricInventory _inventory;
    public CubeObject(RigidBody3D baseNode) : base(baseNode, "cube_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<IShovable>((_,_) => true, Shove);
        RegisterArbitraryAction("Get First Item", 5, (_,_) => !GetInventory().IsEmpty(), (_,ev) => {
            if (ev is not KeyPressEvent) return;
            VolumetricInventory inv = GetInventory().GetAs<VolumetricInventory>();
            string firstJson = inv.GetContents().First();
            RemoveItem(firstJson);
        });
        RegisterArbitraryAction("Store First Item",10,(actor, _) => actor is IContainer actorContainer && !actorContainer.GetInventory().IsEmpty(), (actor, ev) => {
            if (ev is not KeyPressEvent) return;
            IContainer actorContainer = (IContainer)actor;
            string firstJson = actorContainer.GetInventory().GetContents().First();
            string objectMetaTag = Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.META_TAG, firstJson);
            bool success = GetInventory().GetAs<VolumetricInventory>().AddItem(objectMetaTag, firstJson);
            if (!success) {
                Toast.Error((Player)actor, "This inventory is full!");
                return;
            }
            actorContainer.RemoveItem(firstJson);
        });

        _inventory = new VolumetricInventory(350.0f, this);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Shove(ActorBase actorBase, IEventBase ev) => ShoveActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => Items.STORAGE_CRATE.GetItemName();
    public override string GetContext() => $"Contains: {_inventory.GetUsedSize()}/{_inventory.GetMaxSize()} kg";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
    public IInventory GetInventory() => _inventory;
    public bool StoreItem(IObjectBase objectBase, Node node) {
        bool added = GetInventory().GetAs<VolumetricInventory>().AddItem(objectBase);
        if (added) node.QueueFree();
        return added;
    }
    public bool RemoveItem(string objectJson) {
        VolumetricInventory inv = GetInventory().GetAs<VolumetricInventory>();
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
        inv.RemoveItem(obj.Object.GetObjectTag(), objectJson);
        return true;
    }
}