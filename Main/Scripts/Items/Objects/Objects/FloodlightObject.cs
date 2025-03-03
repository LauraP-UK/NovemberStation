using System;
using Godot;

public class FloodlightObject : ObjectBase<RigidBody3D>, IGrabbable, IUsable {
    private readonly SpotLight3D _light;
    private readonly MeshInstance3D _lightTip;

    private bool _isOn;
    private int _powerSeconds = 60;

    private const string
        SPOTLIGHT_PATH = "SpotLight3D",
        LIGHT_TIP_PATH = "Light";

    public FloodlightObject(RigidBody3D baseNode) : base(baseNode, "floodlight_obj", "floodlight_obj") {
        RegisterAction<IGrabbable>((_, _) => true, Grab);
        RegisterAction<IUsable>((_, _) => true, Use);

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

        SetOn(false);
        Scheduler.ScheduleRepeating(0L, 1000L, task => {
            if (GameUtils.IsNodeInvalid(_lightTip)) {
                task.Cancel();
                return;
            }
            if (!_isOn) return;
            _powerSeconds--;
            if (_powerSeconds != 0) return;
            SetOn(false);
            task.Cancel();
        });
    }

    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public void Use(ActorBase actorBase, IEventBase ev) {
        if (ev is not KeyPressEvent) return;

        if (_powerSeconds == 0) {
            Toast.Error((Player)actorBase, "Floodlight is out of power!");
            return;
        }

        SetOn(!_isOn);
    }

    private void SetOn(bool isOn) {
        _isOn = isOn;
        _light.Visible = isOn;
        if (_lightTip.MaterialOverride is not StandardMaterial3D mat) {
            GD.PrintErr("WARN: FloodlightObject.SetOn : Failed to get material override.");
            return;
        }

        if (_isOn) {
            mat.AlbedoColor = Colors.White;
            mat.EmissionEnergyMultiplier = 50;
        }
        else {
            mat.AlbedoColor = Colors.Black;
            mat.EmissionEnergyMultiplier = 0;
        }
    }
}