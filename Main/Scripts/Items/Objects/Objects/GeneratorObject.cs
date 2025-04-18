
using System;
using Godot;

public class GeneratorObject : ObjectBase<Node3D>, IProcess {
    public const string IS_ON_KEY = "isOn";
    [SerialiseData(IS_ON_KEY, nameof(SetMode), nameof(SetMode))]
    private bool _isOn;
    
    public const string FUEL_AMOUNT_KEY = "fuelAmount";
    [SerialiseData(FUEL_AMOUNT_KEY, nameof(SetFuelAmount), nameof(SetFuelDefault))]
    private int _fuelAmount = 100;

    private float _time;
    
    private const string BUTTON_PATH = "Button", FUEL_INPUT_PATH = "FuelInput";
    
    public GeneratorObject(Node3D baseNode, bool dataOnly = false) : base(baseNode, "generator_obj") {
        if (dataOnly) return;

        Area3D button = FindNode<Area3D>(BUTTON_PATH);
        Area3D fuelInput = FindNode<Area3D>(FUEL_INPUT_PATH);

        AddInteractionZone(
            InteractionZoneBuilder<Area3D, GeneratorObject>.Builder("ToggleButton", button, this)
                .WithDisplayName("Toggle Generator")
                .WithContext(() => _isOn ? "Turn Off" : "Turn On")
                .WithAction<IUsable>((_, _) => true, (_, ev) => {
                    if (ev is not KeyPressEvent) return;
                    SetMode(!_isOn);
                })
                .Build()
            );
        AddInteractionZone(
            InteractionZoneBuilder<Area3D, GeneratorObject>.Builder("FuelInput", fuelInput, this)
                .WithDisplayName("Fuel Port")
                .WithContext(() => $"Fuel: {GetFuelAmount()}%")
                .WithAction<IUsable>((_, _) => true, (_, ev) => {
                    if (ev is not KeyPressEvent) return;
                    SetFuelAmount(Math.Min(100, _fuelAmount + 10));
                })
                .Build()
            );
    }

    public void SetMode(bool isOn = false) => _isOn = isOn;
    public void SetFuelAmount(int amount) => _fuelAmount = amount;
    public void SetFuelDefault() => _fuelAmount = 100;
    public override bool DisplayContextMenu() => false;
    public override string GetDisplayName() => "";
    public override string GetContext() => "";
    public override string GetSummary() => "";
    public float GetFuelAmount() => _fuelAmount;
    public void Process(float delta) {
        if (!_isOn) return;
        _time += delta;
        if (_time >= 1.0f) {
            _time = 0.0f;
            _fuelAmount -= 1;
            if (_fuelAmount <= 0) {
                SetMode(false);
                Toast.Error(GameManager.GetPlayer(), "Generator is out of fuel!");
            }
        }
    }
}