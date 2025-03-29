using Godot;

public class GasCanObject : ObjectBase<RigidBody3D>, IGrabbable, IShovable, IDrinkable, ICollectable, IVolumetricObject {
    public const string FUEL_AMOUNT_KEY = "fuelAmount";
    [SerialiseData(FUEL_AMOUNT_KEY, nameof(SetFuelAmount), nameof(SetFuelDefault))]
    private int _fuelAmount = 100;

    public GasCanObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "gascan_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<IShovable>((_,_) => true, Shove);
        RegisterAction<IDrinkable>((_,_) => _fuelAmount > 0, Drink);
        RegisterAction<ICollectable>((_,_) => true, (actor,ev) => CollectActionDefault.Invoke(actor, this, ev));
        RegisterArbitraryAction("Refill", 5, (_,_) => _fuelAmount <= 0, (_,ev) => {
            if (ev is not KeyPressEvent) return;
            _fuelAmount = 100;
        });
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Shove(ActorBase actorBase, IEventBase ev) => ShoveActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Drink(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent && ev is not MouseInputEvent) return;
        _fuelAmount -= 10;
    }

    public override string GetDisplayName() => Items.GAS_CAN.GetItemName();
    public override string GetContext() {
        return _fuelAmount switch {
            > 0 => $"Fuel Type: Gasoline\nFuel Remaining: {_fuelAmount}",
            _ => "EMPTY"
        };
    }
    public void SetFuelAmount(int amount) => _fuelAmount = amount;
    public void SetFuelDefault() => _fuelAmount = 100;
    
    public override string GetSummary() => GetContext().Replace("\n", " | ");
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);
    public float GetSize() => 15.0f;
}