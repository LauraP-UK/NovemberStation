using Godot;

public class BatteryObject : ObjectBase<RigidBody3D>, IGrabbable, ICollectable, IVolumetricObject {
    private const float MAX_POWER = 100;
    
    public const string POWER_KEY = "power";
    [SerialiseData(POWER_KEY, nameof(SetBatteryPower), nameof(ResetBatteryPower))]
    private float _power = 100.0f;

    public BatteryObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "battery_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<ICollectable>((_,_) => true, Collect);
    }
    public override string GetDisplayName() => Items.BATTERY.GetItemName();
    public override string GetContext() => $"{GetBatteryPower()}%";
    public override string GetSummary() => $"Power: {GetBatteryPower()}%";
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);
    public float GetBatteryPower() => Mathsf.Round(_power, 2);
    public void SetBatteryPower(float power) => _power = power;
    private void ResetBatteryPower() => _power = MAX_POWER;
    public float GetMaxPower() => MAX_POWER;
    public float GetSize() => 0.2f;
}