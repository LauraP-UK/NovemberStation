using System;
using System.Collections.Generic;
using Godot;

public class ItemType {
    private readonly string _typeID, _itemName, _imagePath, _modelPath, _description;
    private readonly int _itemCost;
    private readonly HeldDisplaySettings _displaySettings;
    private readonly Action<RigidBody3D> _onNodeSpawn;
    private readonly Action<IObjectBase> _onDataSpawn;

    private static readonly List<KeyValuePair<int, bool>>
        _collisionMasks = new() {
            new KeyValuePair<int, bool>(1, true),
            new KeyValuePair<int, bool>(2, true),
            new KeyValuePair<int, bool>(3, true),
            new KeyValuePair<int, bool>(4, true)
        },
        _collisionLayers = new() {
            new KeyValuePair<int, bool>(3, true),
            new KeyValuePair<int, bool>(4, true)
        },
        _noCollisionMasks = new() {
            new KeyValuePair<int, bool>(1, false),
            new KeyValuePair<int, bool>(2, false),
            new KeyValuePair<int, bool>(3, false),
            new KeyValuePair<int, bool>(4, false)
        },
        _noCollisionLayers = new() {
            new KeyValuePair<int, bool>(3, false),
            new KeyValuePair<int, bool>(4, false)
        };

    private ItemType(string typeID, string itemName, string imagePath, string modelPath, string description, int itemCost, HeldDisplaySettings displaySettings,
        Action<RigidBody3D> onNodeSpawn, Action<IObjectBase> onDataSpawn) {
        _typeID = typeID;
        _itemName = itemName;
        _imagePath = imagePath;
        _modelPath = modelPath;
        _description = description;
        _itemCost = itemCost;
        _displaySettings = displaySettings ?? HeldDisplaySettings.Default();
        _onNodeSpawn = onNodeSpawn;
        _onDataSpawn = onDataSpawn;
    }

    public string GetTypeID() => _typeID;
    public string GetItemName() => _itemName;
    public string GetImagePath() => _imagePath;
    public string GetModelPath() => _modelPath;
    public string GetDescription() => _description;
    public int GetItemCost() => _itemCost;
    public Texture2D GetImage() => ResourceLoader.Load<Texture2D>(GetImagePath());

    public void TryOnDataSpawn(IObjectBase objBase) => _onDataSpawn?.Invoke(objBase);
    public void ApplyHeldOrientation(Node3D node) => _displaySettings.ApplyTo(node);

    public RigidBody3D CreateInstance() {
        RigidBody3D rigidBody3D = Loader.SafeInstantiate<RigidBody3D>(GetModelPath());
        SetCollision(true, rigidBody3D);
        _onNodeSpawn?.Invoke(rigidBody3D);
        return rigidBody3D;
    }

    public void SetCollision(bool collision, RigidBody3D node) {
        (collision ? _collisionLayers : _noCollisionLayers).ForEach(pair => node.SetCollisionLayerValue(pair.Key, pair.Value));
        (collision ? _collisionMasks : _noCollisionMasks).ForEach(pair => node.SetCollisionMaskValue(pair.Key, pair.Value));
    }

    public ShopItemDisplayButton CreateButton() {
        ShopItemDisplayButton item = new(this);
        item.SetName(GetItemName());
        item.SetDescription(GetDescription());
        item.SetCost(GetItemCost());
        item.SetHeight(100);
        item.SetTexture(GetImage());
        item.OnPressed(_ => GD.Print($"INFO: PlayerController.OnOpenShop() : Button pressed! Name: {GetItemName()}  Cost: {GetItemCost()}"));
        item.GetButton().OnButtonDown(_ => item.VisualPress(true));
        item.GetButton().OnButtonUp(_ => item.VisualPress(false));
        return item;
    }

    public static ItemType Create(string typeID, string itemName, string imagePath, string modelPath, string description, int itemCost,
        HeldDisplaySettings displaySettings = null, Action<RigidBody3D> onNodeSpawn = null, Action<IObjectBase> onDataSpawn = null) {
        return new ItemType(typeID, itemName, imagePath, modelPath, description, itemCost, displaySettings, onNodeSpawn, onDataSpawn);
    }
}