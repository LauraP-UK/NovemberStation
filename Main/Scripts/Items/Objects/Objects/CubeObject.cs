
using System.Linq;
using Godot;

public class CubeObject : ObjectBase<RigidBody3D>, IGrabbable, IShovable, IContainer {
    private readonly VolumetricInventory _inventory;
    public CubeObject(RigidBody3D baseNode) : base(baseNode, "cube_obj", "cube_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<IShovable>((_,_) => true, Shove);
        RegisterArbitraryAction("Get First Item", 5, (_,_) => GetVolumetricInventory().GetUsedSize() > 0, (_,ev) => {
            if (ev is not KeyPressEvent) return;
            VolumetricInventory inv = GetVolumetricInventory();
            string firstJson = inv.GetContents().First();
            RemoveItem(firstJson);
        });
        RegisterArbitraryAction("Store First Item",10,(actor, _) => actor is IContainer actorContainer && !actorContainer.GetInventory().IsEmpty(), (actor, ev) => {
            if (ev is not KeyPressEvent) return;
            IContainer actorContainer = (IContainer)actor;
            string firstJson = actorContainer.GetInventory().GetContents().First();
            string objectMetaTag = Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.META_TAG, firstJson);
            bool success = GetVolumetricInventory().AddItem(objectMetaTag, firstJson);
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

    public override string GetDisplayName() => "Storage Crate";
    public override string GetContext() => $"Contains: {_inventory.GetUsedSize()}/{_inventory.GetMaxSize()} kg";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() => new();
    public IInventory GetInventory() => _inventory;
    public bool StoreItem(IObjectBase objectBase, Node node) {
        bool added = GetVolumetricInventory().AddItem(objectBase);
        if (added) {
            GD.Print($"Freeing node {node.Name}  |  {node.GetType()}  |  {objectBase.GetObjectTag()}");
            node.QueueFree();
        }
        Scheduler.ScheduleOnce(1000, _ => GD.Print($"Node is valid: {!GameUtils.IsNodeInvalid(node)}"));
        return added;
    }
    public bool RemoveItem(string objectJson) {
        VolumetricInventory inv = GetVolumetricInventory();
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
    public VolumetricInventory GetVolumetricInventory() => _inventory;
}