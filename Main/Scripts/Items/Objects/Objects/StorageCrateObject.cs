using Godot;

public class StorageCrateObject : ObjectBase<RigidBody3D>, IGrabbable, IShovable, IContainer {
    [InventorySerialiseData]
    private readonly VolumetricInventory _inventory;

    public StorageCrateObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "cube_obj") {
        _inventory = new VolumetricInventory(350.0f, this);
        if (dataOnly) return;
        
        RegisterAction<IGrabbable>((_, _) => true, Grab);
        RegisterAction<IShovable>((_, _) => true, Shove);
        RegisterArbitraryAction("Open", 20, (_, _) => true, (_, ev) => {
            if (ev is not KeyPressEvent) return;
            DualInvDisplayMenu dualInvDisplayMenu = new();
            dualInvDisplayMenu.ModifyForm(form => form.SetOtherInventory(this));
            dualInvDisplayMenu.Open();
        });
        RegisterArbitraryAction("Show Serialised Data", 30, (_, _) => _inventory.GetUsedSize() > 0.0f, (_, ev) => {
            if (ev is not KeyPressEvent) return;
            string serialise = GetInventory().Serialise();
            GD.Print($"Crate serialised data: {serialise}");
            GetInventory().Deserialise(serialise);

            GD.Print(serialise);
        });
    }

    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Shove(ActorBase actorBase, IEventBase ev) => ShoveActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public override string GetDisplayName() => Items.STORAGE_CRATE.GetItemName();
    public override string GetContext() => $"Contains: {_inventory.GetUsedSize()}/{_inventory.GetMaxSize()} kg";
    public override string GetSummary() => GetContext();
    public IInventory GetInventory() => _inventory;
    public string GetName() => GetDisplayName();
    public AddItemFailCause StoreItem(ItemType itemType) {
        RigidBody3D node = itemType.CreateInstance();
        IObjectBase obj = GameManager.I().RegisterObject(node);
        AddItemFailCause result = StoreItem(obj, node);
        node.QueueFree();
        return result;
    }

    public AddItemFailCause StoreItem(IObjectBase objectBase, Node node) {
        AddItemFailCause result = _inventory.AddItem(objectBase);
        if (result == AddItemFailCause.SUCCESS) node.QueueFree();
        return result;
    }

    public AddItemFailCause StoreItem(string objectMetaTag, string objectJson) => GetInventory().AddItem(objectMetaTag, objectJson);

    public bool RemoveItem(string objectJson) {
        VolumetricInventory inv = GetInventory().GetAs<VolumetricInventory>();
        IObjectBase obj = ObjectAtlas.DeserialiseDataWithoutNode(objectJson);
        inv.RemoveItem(obj.GetObjectTag(), objectJson);
        return true;
    }
}