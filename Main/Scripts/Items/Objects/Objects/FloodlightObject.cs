using System;
using Godot;

public class FloodlightObject : ObjectBase<RigidBody3D>, IGrabbable, IUsable, IProcess {
    private readonly SpotLight3D _light;
    private readonly MeshInstance3D _lightTip;

    private const long MAX_POWER_MILLIS = 60000 * 2L;
    private const float FAIL_START_AT_PERCENT = 16.6f;
    private readonly float _initialRange, _initialAngle, _initialEnergy;

    private bool _isOn;
    private long _powerMillis = MAX_POWER_MILLIS;

    private const string
        SPOTLIGHT_PATH = "SpotLight3D",
        LIGHT_TIP_PATH = "Light";

    public FloodlightObject(RigidBody3D baseNode) : base(baseNode, "floodlight_obj", "floodlight_obj") {
        RegisterAction<IGrabbable>((_, _) => true, Grab);
        RegisterAction<IUsable>((_, _) => true, Use);
        RegisterArbitraryAction("Recharge", 10, (_,_) => _powerMillis <= 0, Recharge);
        RegisterArbitraryAction("Save to File", 20, (_,_) => true, SerialiseTest);

        string finding = "NULL";
        try {
            finding = SPOTLIGHT_PATH;
            _light = FindNode<SpotLight3D>(SPOTLIGHT_PATH);
            finding = LIGHT_TIP_PATH;
            _lightTip = FindNode<MeshInstance3D>(LIGHT_TIP_PATH);
        }
        catch (Exception e) {
            GD.PrintErr($"WARN: FloodlightObject.<init> : Failed to find required {finding} node.");
            return;
        }

        _lightTip.MaterialOverride = (Material) _lightTip.MaterialOverride.Duplicate();
        _initialAngle = _light.SpotAngle;
        _initialRange = _light.SpotRange;
        _initialEnergy = _light.LightEnergy;

        ToggleLight(false);
    }

    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public void Use(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;

        if (_powerMillis <= 0) {
            Toast.Error((Player)actorBase, "Floodlight is out of power!");
            return;
        }

        ToggleLight(!_isOn);
    }

    public void Recharge(ActorBase actorBase, IEventBase ev) => _powerMillis = MAX_POWER_MILLIS;
    private void SerialiseTest(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;
        string savePath = "user://SerialiseTest.json";
        string jsonData = Serialize(); // from your IObjectBase

        using FileAccess file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
        file.StoreString(jsonData);
        file.Close();
        GD.Print($"Serialised to {ProjectSettings.GlobalizePath(savePath)}");
    }

    private void ToggleLight(bool isOn) {
        _isOn = isOn;
        _light.Visible = isOn;
        HandleLighting();
    }

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
        }
        else {
            mat.AlbedoColor = Colors.Black;
            mat.EmissionEnergyMultiplier = 0;
        }
    }

    public void Process(float delta) {
        if (GameUtils.IsNodeInvalid(_lightTip) || !_isOn || _powerMillis <= 0) return;
        
        _powerMillis -= (long)(delta * 1000.0f);
        _powerMillis = Math.Max(0, _powerMillis);
        HandleLighting();

        if (_powerMillis <= 0) ToggleLight(false);
    }

    private float GetPowerRemaining() => Mathsf.Round(Mathsf.Remap(MAX_POWER_MILLIS, 0f, _powerMillis, 100f, 0f), 2);

    public override string GetDisplayName() => "Floodlight";
    public override string GetContext() {
        string secondLine = _powerMillis <= 0 ? "NO POWER" : $"Power: {GetPowerRemaining():00.00}%";
        return $"Status: {(_isOn ? "ON" : "OFF")}\n{secondLine}";
    }

    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() {
        return new SmartDictionary<string, SmartSerialData> {
            {"isOn", SmartSerialData.From(_isOn, v => ToggleLight(Convert.ToBoolean(v)), () => ToggleLight(false))},
            {"powerMillis", SmartSerialData.From(_powerMillis, v => _powerMillis = Convert.ToInt64(v), () => _powerMillis = MAX_POWER_MILLIS)}
        };
    }
}