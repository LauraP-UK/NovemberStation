using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class FloodlightObject : ObjectBase<RigidBody3D>, IGrabbable, IUsable, ICollectable, IProcess, IVolumetricObject, IContainer {
    private readonly SpotLight3D _light;
    private readonly MeshInstance3D _lightTip;

    private const float FAIL_START_AT_PERCENT = 16.6f;

    private readonly float _initialRange, _initialAngle, _initialEnergy;
    
    [InventorySerialise]
    private readonly QuantitativeInventory _inventory;

    public const string IS_ON_KEY = "isOn";
    
    [SerialiseData(IS_ON_KEY, nameof(ToggleLight), nameof(TurnOff))]
    private bool _isOn;

    private const string
        SPOTLIGHT_PATH = "SpotLight3D",
        LIGHT_TIP_PATH = "Light";

    public FloodlightObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "floodlight_obj") {
        _inventory = new QuantitativeInventory(4, this);
        _inventory.AddFilter(FilterHelper.MakeItemTypeFilter(Items.BATTERY));

        if (dataOnly) return;

        RegisterAction<IGrabbable>((_, _) => true, Grab);
        RegisterAction<IUsable>((_, _) => true, Use);
        RegisterAction<ICollectable>((_, _) => true, Collect);
        RegisterArbitraryAction("Open Battery Slot", 10, (_, _) => true, Recharge);
        RegisterArbitraryAction("Save to File", 20, (_, _) => true, SerialiseTest);

        string finding = "NULL";
        try {
            finding = SPOTLIGHT_PATH;
            _light = FindNode<SpotLight3D>(SPOTLIGHT_PATH);
            finding = LIGHT_TIP_PATH;
            _lightTip = FindNode<MeshInstance3D>(LIGHT_TIP_PATH);
        } catch (Exception e) {
            GD.PrintErr($"WARN: FloodlightObject.<init> : Failed to find required {finding} node.");
            return;
        }

        _lightTip.MaterialOverride = (Material)_lightTip.MaterialOverride.Duplicate();
        _initialAngle = _light.SpotAngle;
        _initialRange = _light.SpotRange;
        _initialEnergy = _light.LightEnergy;

        ToggleLight(false);
    }

    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public void Use(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;

        if (GetPowerRemaining() <= 0.0f) {
            Toast.Error((Player)actorBase, "Floodlight is out of power!");
            return;
        }

        int missingBatteries = _inventory.GetRemainingSlots();
        if (missingBatteries != 0) {
            Toast.Error((Player)actorBase, $"Missing {missingBatteries} Batter{(missingBatteries == 1 ? "y" : "ies")}!");
            return;
        }

        ToggleLight(!_isOn);
    }

    public void Recharge(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;
        if (actorBase is not IContainer contActor) return;
        
        ToggleLight(false);

        bool hasBattery = contActor.GetInventory().HasItem(Items.BATTERY);
        if (!hasBattery && GetInventory().GetContents().Count == 0) {
            Toast.Error((Player)actorBase, "You need a battery to recharge the floodlight.");
            return;
        }

        DualInvDisplayMenu dualInvDisplayMenu = new();
        dualInvDisplayMenu.ModifyForm(form => form.SetOtherInventory(this));
        dualInvDisplayMenu.Open();
    }

    private void SerialiseTest(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;
        string savePath = "user://SerialiseTest.json";

        List<string> contents = _inventory.GetContents();
        GD.Print($"Serialising these contents: {string.Join(", ", contents)}");

        string jsonData = Serialise();

        using FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
        file.StoreString(jsonData);
        file.Close();
        GD.Print($"Serialised to {ProjectSettings.GlobalizePath(savePath)}");
    }

    private void ToggleLight(bool isOn) {
        _isOn = isOn;
        if (_light == null) return;
        _light.Visible = isOn;
        HandleLighting();
    }

    private void TurnOff() => ToggleLight(false);

    private void HandleLighting() {
        if (_lightTip.MaterialOverride is not StandardMaterial3D mat) {
            GD.PrintErr("WARN: FloodlightObject.HandleLighting() : Failed to get material override.");
            return;
        }

        if (_isOn) {
            float fadeRatio = 1.0f - Mathsf.InverseLerpClamped(0.0f, FAIL_START_AT_PERCENT, GetPowerRemaining());
            float colourValue = Mathsf.Round(1.0f - fadeRatio, 4);
            mat.AlbedoColor = new Color(colourValue, colourValue, colourValue);
            mat.EmissionEnergyMultiplier = Mathsf.Round(Mathsf.Lerp(50.0f, 0.0f, fadeRatio), 4);
            _light.SpotAngle = Mathsf.Round(Mathsf.Lerp(_initialAngle, _initialAngle * 0.33f, fadeRatio), 4);
            _light.SpotRange = Mathsf.Round(Mathsf.Lerp(_initialRange, _initialRange * 0.5f, fadeRatio), 4);
            _light.LightEnergy = Mathsf.Round(Mathsf.Lerp(_initialEnergy, _initialEnergy * 0.5f, fadeRatio), 4);
        } else {
            mat.AlbedoColor = Colors.Black;
            mat.EmissionEnergyMultiplier = 0;
        }
    }

    public void Process(float delta) {
        if (GameUtils.IsNodeInvalid(_lightTip) || !_isOn || GetPowerRemaining() <= 0.0f) return;

        List<string> oldJsons = _inventory.GetContents();
        List<string> newJsons = oldJsons.Select(json => Serialiser.ModifySpecificData<float>(BatteryObject.POWER_KEY, p => Math.Max(p - (delta * 0.5f), 0.0f), json)).ToList();

        string tag = Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.META_TAG, newJsons[0]);

        _inventory.ClearContents();
        foreach (string newJson in newJsons) _inventory.AddItem(tag, newJson);
        HandleLighting();
        if (GetPowerRemaining() <= 0.0f) ToggleLight(false);
    }

    private float GetPowerRemaining() {
        float totalCount =
            GetInventory()
            .GetContents()
            .Select(json => Serialiser.GetSpecificData<float>(BatteryObject.POWER_KEY, json))
            .Sum();
        
        float maxPower =
            ((BatteryObject)ObjectAtlas.CreateObject(typeof(BatteryObject), null)).GetMaxPower() *
            _inventory.GetMaxQuantity();
        
        return Mathsf.Round(Mathsf.Remap(0f, maxPower, totalCount, 0f, 100f), 2);
    }

    public override string GetDisplayName() => Items.FLOODLIGHT.GetItemName();

    public override string GetContext() {
        float power = GetPowerRemaining();
        string secondLine = power <= 0.0f ? "NO POWER" : $"Power: {power:00.00}%";
        return $"Status: {(_isOn ? "ON" : "OFF")}\n{secondLine}";
    }

    public override string GetSummary() => GetContext().Replace("\n", " | ");

    public float GetSize() => 90.0f;
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);
    public IInventory GetInventory() => _inventory;
    public string GetName() => "Floodlight Battery Slots";

    public AddItemFailCause StoreItem(IObjectBase objectBase, Node node) {
        AddItemFailCause result = GetInventory().GetAs<QuantitativeInventory>().AddItem(objectBase);
        if (result == AddItemFailCause.SUCCESS) node.QueueFree();
        return result;
    }

    public AddItemFailCause StoreItem(string objectMetaTag, string objectJson) => GetInventory().AddItem(objectMetaTag, objectJson);

    public bool RemoveItem(string objectJson) {
        QuantitativeInventory inv = GetInventory().GetAs<QuantitativeInventory>();
        IObjectBase obj = ObjectAtlas.DeserialiseDataWithoutNode(objectJson);
        inv.RemoveItem(obj.GetObjectTag(), objectJson);
        return true;
    }
}