using System;
using Godot;

public class BatteryObject : ObjectBase<RigidBody3D>, IGrabbable, ICollectable, IVolumetricObject {
    private const float MAX_POWER = 100;
    private float _power = MAX_POWER;

    public BatteryObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "battery_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_, _) => true, Grab);
        RegisterAction<ICollectable>((_,_) => true, Collect);
    }
    public override string GetDisplayName() => Items.BATTERY.GetItemName();
    public override string GetContext() => $"{GetBatteryPower()}%";
    public override SmartDictionary<string, SmartSerialData> GetSerialiseData() {
        return new SmartDictionary<string, SmartSerialData> {
            { "power", SmartSerialData.From(_power, v => _power = Convert.ToSingle(v), () => _power = MAX_POWER) }
        };
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);
    public float GetBatteryPower() => Mathsf.Round(_power, 2);

    public float GetSize() => 0.2f;
}