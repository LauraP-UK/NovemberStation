
using Godot;

public class GasCanObject : ObjectBase<RigidBody3D> {

    private int _fuelAmount = 100;
    
    public GasCanObject(RigidBody3D baseNode) : base(baseNode) {
        GD.Print($"GasCanObject created for {baseNode.Name}");
    }
    
    public void DrinkFrom() {
        if (_fuelAmount <= 0) {
            GD.Print("Gas can is empty.");
            return;
        }
        _fuelAmount -= 10;
        GD.Print($"Drinking from gas can. Fuel remaining: {_fuelAmount}");
    }

    public new static string GetObjectTag() => "gascan_obj";
}