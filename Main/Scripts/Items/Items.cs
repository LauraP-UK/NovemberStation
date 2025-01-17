using System;
using System.Collections.Generic;
using System.Linq;

namespace NovemberStation.Main.Scripts.Items;

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
    public static readonly ItemType BLUE_CUBE = ItemType.Create(
        "Blue Cube",
        "res://Main/Textures/Items/BlueCube.png",
        "res://Main/Prefabs/Sandbox/Geometry/PhysicsCube.tscn",
        "A mysterious blue cube.",
        130,
        cube => cube.Mass = Math.Max(Randf.Random(0, 10) * 5, 1));
    
    private static readonly ItemType[] ALL_ITEMS = {GAS_CAN, WORK_DESK, BLUE_CUBE};
    public static List<ItemType> GetItems() => new(ALL_ITEMS);
    public static List<ShopItemDisplayButton> GetItemButtons() => GetItems().Select(item => item.CreateButton()).ToList();
}