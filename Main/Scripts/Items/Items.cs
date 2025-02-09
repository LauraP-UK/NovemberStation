using System;
using System.Collections.Generic;
using System.Linq;

public class Items {
    
    public static readonly ItemType GAS_CAN = ItemType.Create(
        "Gas Can",
        "res://Main/Textures/Items/GasCanImage.png",
        "res://Main/Prefabs/Sandbox/GasCanTest.tscn",
        "A can of gasoline.",
        50);
    public static readonly ItemType WORK_DESK = ItemType.Create(
        "Work Desk",
        "res://Main/Textures/Items/DeskImage.png",
        "res://Main/Prefabs/Sandbox/DeskTest.tscn",
        "A work desk.",
        250);
    public static readonly ItemType STORAGE_CRATE = ItemType.Create(
        "Storage Crate",
        "res://Main/Textures/Items/StorageCrate.png",
        "res://Main/Prefabs/Sandbox/Geometry/PhysicsCube.tscn",
        "A wooden storage crate.",
        130,
        cube => cube.Mass = Math.Max(Randf.Random(0, 10) * 5, 1));
    public static readonly ItemType CROWBAR = ItemType.Create(
        "Crowbar",
        "res://Main/Textures/Items/Crowbar.png",
        "res://Main/Prefabs/PhysicsObjects/Crowbar.tscn",
        "Gordon Freeman, for someone who proclaims to be a doctor, you should know that not every peepee time is a poopoo time. But every peepee-poo-pu- every... fuck.",
        300);
    public static readonly ItemType FIRE_EXTINGUISHER = ItemType.Create(
        "Fire Extinguisher",
        "res://Main/Textures/Items/FireExtinguisher.png",
        "res://Main/Prefabs/PhysicsObjects/FireExtinguisher.tscn",
        "Set things on unfire.",
        150);
    
    private static readonly ItemType[] ALL_ITEMS = {GAS_CAN, WORK_DESK, STORAGE_CRATE, CROWBAR, FIRE_EXTINGUISHER};
    public static List<ItemType> GetItems() => new(ALL_ITEMS);
    public static List<ShopItemDisplayButton> GetItemButtons() => GetItems().Select(item => item.CreateButton()).ToList();
    
    public static ShopItemDisplayButton GetCloseButton() {
        ShopItemDisplayButton closeButton = new("CloseButton");
        closeButton.GetCostLabel().GetElement().Visible = false;
        closeButton.GetDescLabel().GetElement().Visible = false;
        closeButton.SetName("Close");
        closeButton.SetCost(0);
        closeButton.SetHeight(100);
        closeButton.GetButton().OnButtonDown(btn => closeButton.VisualPress(true));
        closeButton.GetButton().OnButtonUp(btn => closeButton.VisualPress(false));
        return closeButton;
    }
}