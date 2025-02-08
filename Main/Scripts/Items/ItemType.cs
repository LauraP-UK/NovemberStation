using System;
using System.Collections.Generic;
using Godot;

public class ItemType {
    private readonly string _itemName, _imagePath, _modelPath, _description;
    private readonly int _itemCost;
    private readonly Action<RigidBody3D> _onSpawn;

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
        };

    private ItemType(string itemName, string imagePath, string modelPath, string description, int itemCost, Action<RigidBody3D> onSpawn) {
        _itemName = itemName;
        _imagePath = imagePath;
        _modelPath = modelPath;
        _description = description;
        _itemCost = itemCost;
        _onSpawn = onSpawn;
    }

    public string GetItemName() => _itemName;
    public string GetImagePath() => _imagePath;
    public string GetModelPath() => _modelPath;
    public string GetDescription() => _description;
    public int GetItemCost() => _itemCost;

    public Texture2D GetImage() => ResourceLoader.Load<Texture2D>(GetImagePath());

    public RigidBody3D CreateInstance() {
        RigidBody3D rigidBody3D = Loader.SafeInstantiate<RigidBody3D>(GetModelPath());
        _collisionLayers.ForEach(pair => rigidBody3D.SetCollisionLayerValue(pair.Key, pair.Value));
        _collisionMasks.ForEach(pair => rigidBody3D.SetCollisionMaskValue(pair.Key, pair.Value));
        _onSpawn?.Invoke(rigidBody3D);
        return rigidBody3D;
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

    public static ItemType Create(string itemName, string imagePath, string modelPath, string description, int itemCost, Action<RigidBody3D> onSpawn = null) {
        return new ItemType(itemName, imagePath, modelPath, description, itemCost, onSpawn);
    }
}