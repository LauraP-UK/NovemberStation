
using System;
using Godot;

public class ItemType {
    private readonly string _itemName, _imagePath, _modelPath, _description;
    private readonly int _itemCost;
    private readonly Action<RigidBody3D> _onSpawn;

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
        PackedScene packedScene = ResourceLoader.Load<PackedScene>(GetModelPath());
        RigidBody3D rigidBody3D = packedScene.Instantiate<RigidBody3D>();
        rigidBody3D.SetCollisionLayerValue(3, true);
        rigidBody3D.SetCollisionLayerValue(4, true);
        rigidBody3D.SetCollisionMaskValue(1, true);
        rigidBody3D.SetCollisionMaskValue(2, true);
        rigidBody3D.SetCollisionMaskValue(3, true);
        rigidBody3D.SetCollisionMaskValue(4, true);
        _onSpawn?.Invoke(rigidBody3D);
        return rigidBody3D;
    }
    
    public ShopItemDisplayButton CreateButton() {
        ShopItemDisplayButton item = new(this);
        item.SetName(GetItemName());
        item.SetCost(GetItemCost());
        item.SetHeight(125);
        item.SetTexture(GetImage());
        item.OnPressed(elem => GD.Print($"INFO: PlayerController.OnOpenShop() : Button pressed! Name: {GetItemName()}  Cost: {GetItemCost()}"));
        item.GetButton().OnButtonDown(btn => item.VisualPress(true));
        item.GetButton().OnButtonUp(btn => item.VisualPress(false));
        return item;
    }

    public static ItemType Create(string itemName, string imagePath, string modelPath, string description, int itemCost, Action<RigidBody3D> onSpawn = null) {
        return new ItemType(itemName, imagePath, modelPath, description, itemCost, onSpawn);
    }

}