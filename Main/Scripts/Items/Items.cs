using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

public class Items {
    
    public static readonly ItemType GAS_CAN = ItemType.Create("gas_can",
        "Gas Can",
        "res://Main/Textures/Items/GasCanImage.png",
        "res://Main/Prefabs/Sandbox/GasCanTest.tscn",
        "A can of gasoline.",
        50,
        HeldDisplaySettings.Create(new Vector3(0.147f, -0.056f, -0.112f), new Vector3(6.0f, -146.0f, 5.8f), 0.5f));
    public static readonly ItemType WORK_DESK = ItemType.Create("work_desk",
        "Work Desk",
        "res://Main/Textures/Items/DeskImage.png",
        "res://Main/Prefabs/Sandbox/DeskTest.tscn",
        "A work desk.",
        250);
    public static readonly ItemType STORAGE_CRATE = ItemType.Create("storage_crate",
        "Storage Crate",
        "res://Main/Textures/Items/StorageCrate.png",
        "res://Main/Prefabs/Sandbox/Geometry/PhysicsCube.tscn",
        "A wooden storage crate.",
        130,
        HeldDisplaySettings.Default(),
        crate => crate.Mass = Math.Max(Randf.Random(0, 10) * 5, 1));
    public static readonly ItemType CROWBAR = ItemType.Create("crowbar",
        "Crowbar",
        "res://Main/Textures/Items/Crowbar.png",
        "res://Main/Prefabs/PhysicsObjects/Crowbar.tscn",
        "Gordon Freeman, for someone who proclaims to be a doctor, you should know that not every peepee time is a poopoo time. But every peepee-poo-pu- every... fuck.",
        300,
        HeldDisplaySettings.Create(new Vector3(0.039f, -0.054f, 0.012f), new Vector3(68.9f, -42.6f, -4.1f), 0.66f));
    public static readonly ItemType FIRE_EXTINGUISHER = ItemType.Create("fire_extinguisher",
        "Fire Extinguisher",
        "res://Main/Textures/Items/FireExtinguisher.png",
        "res://Main/Prefabs/PhysicsObjects/FireExtinguisher.tscn",
        "Set things on unfire.",
        150,
        HeldDisplaySettings.Create(new Vector3(0.039f, -0.054f, 0.012f), new Vector3(-10.2f, -79.4f, 22.8f), 0.66f));
    public static readonly ItemType FLOODLIGHT = ItemType.Create("floodlight",
        "Floodlight",
        "res://Main/Textures/Items/Floodlight.png",
        "res://Main/Prefabs/PhysicsObjects/Floodlight.tscn",
        "I love lamp.",
        300,
        HeldDisplaySettings.Create(new Vector3(0.0f, 0.01f, 0.06f), new Vector3(-90.0f, 4.0f, 0.0f), 0.75f),
        null,
        data => {
            for (int i = 0; i < 4; i++) ((FloodlightObject)data).StoreItem(BATTERY);
        });
    public static readonly ItemType BED = ItemType.Create("bed",
        "Bed",
        "res://Main/Textures/Items/BedImage.png",
        "res://Main/Prefabs/PhysicsObjects/Bed.tscn",
        "Eepy weepy ready for sleepy.",
        1000);
    public static readonly ItemType DIGITAL_CLOCK = ItemType.Create("digital_clock",
        "Digital Clock",
        "res://Main/Textures/Items/ClockImage.png",
        "res://Main/Prefabs/PhysicsObjects/DigitalClock.tscn",
        "Letting the days go by. Let the water hold me down. Letting the days go by. Water flowing underground.",
        250,
        HeldDisplaySettings.Create(new Vector3(0.109f, 0.04f, -0.14f), new Vector3(2.2f, -22.9f, 5.8f), 0.66f));
    public static readonly ItemType BATTERY = ItemType.Create("battery",
        "Battery",
        "res://Main/Textures/Items/BatteryImage.png",
        "res://Main/Prefabs/PhysicsObjects/Battery.tscn",
        "Do not eat.",
        25,
        HeldDisplaySettings.Create(new Vector3(0.01f, 0.007f, -0.054f), new Vector3(0.2f, -25.0f, -4.9f), 0.75f));
    public static readonly ItemType PAPER_SHEET = ItemType.Create("paper_sheet",
        "Paper Sheet",
        "res://Main/Textures/Items/PaperSheetImage.png",
        "res://Main/Prefabs/PhysicsObjects/PaperSheet.tscn",
        "A sheet of paper.",
        2,
        HeldDisplaySettings.Create(new Vector3(0.0f, 0.06f, 0.0f), new Vector3(90.0f, 33.0f, 77.4f), 0.75f));
    public static readonly ItemType GLOWSTICK = ItemType.Create("glowstick",
        "Glowstick",
        "res://Main/Textures/Items/GlowstickImage.png",
        "res://Main/Prefabs/PhysicsObjects/GlowStick.tscn",
        "I'm your only friend. I'm not your only friend, but I'm a little glowing friend. But really, I'm not actually your friend. But I am.",
        100,
        HeldDisplaySettings.Create(new Vector3(0.0f, 0.06f, 0.0f), new Vector3(168.3f, -44.4f, 0.0f), 0.75f),
        null,
        data => {
            ((GlowstickObject)data).SetRandomColour();
        });
    
    private static readonly ItemType[] ALL_ITEMS;

    static Items() {
        ALL_ITEMS = typeof(Items)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => typeof(ItemType).IsAssignableFrom(field.FieldType))
            .Select(field => field.GetValue(null) as ItemType)
            .Where(p => p != null)
            .ToArray();
        GD.Print("[Items] INFO: Registered all items.");
    }
    public static List<ItemType> GetItems() => new(ALL_ITEMS);
    public static List<ShopItemDisplayButton> GetItemButtons() => GetItems().Select(item => item.CreateButton()).ToList();
    public static ItemType GetViaID(string id) => GetItems().FirstOrDefault(itemType => itemType.GetTypeID() == id);
    public static ItemType GetViaPath(string path) => GetItems().FirstOrDefault(itemType => itemType.GetModelPath() == path);

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