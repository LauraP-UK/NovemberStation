﻿
using Godot;

public class GasCanObject : ObjectBase<RigidBody3D>, IGrabbable, IShovable, IDrinkable {
    private int _fuelAmount = 100;

    public GasCanObject(RigidBody3D baseNode) : base(baseNode, "gascan_obj", "gascan_obj") {
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<IShovable>((_,_) => true, Shove);
        RegisterAction<IDrinkable>((_,_) => _fuelAmount > 0, Drink);
    }
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Shove(ActorBase actorBase, IEventBase ev) => ShoveActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Drink(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;
        _fuelAmount -= 10;
        if (_fuelAmount == 0) Toast.Error((Player)actorBase, "Gas Can is now EMPTY!");
        else Toast.Info((Player)actorBase, $"Drank 10 fuel. Gasoline remaining {_fuelAmount}.");
    }
}