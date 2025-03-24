using System;
using Godot;

public class DigitalClockObject : ObjectBase<RigidBody3D>, IGrabbable, IProcess, IVolumetricObject {
    private const string
        SCREEN_VIEWPORT_PATH = "Screen/ScreenViewport",
        SCREEN_PATH = "Screen";

    private readonly SubViewport _viewport;
    private readonly MeshInstance3D _screen;
    private readonly TimeDisplayMenu _timeMenu;
    
    private int _lastMinute = -1;
    private bool _showDivider = false;

    public DigitalClockObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "digitalclock_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_, _) => true, Grab);
        RegisterAction<ICollectable>((_,_) => true, (actor,ev) => CollectActionDefault.Invoke(actor, this, ev));
        
        string finding = "NULL";
        try {
            finding = SCREEN_VIEWPORT_PATH;
            _viewport = FindNode<SubViewport>(SCREEN_VIEWPORT_PATH);
            finding = SCREEN_PATH;
            _screen = FindNode<MeshInstance3D>(SCREEN_PATH);
        }
        catch (Exception e) {
            GD.PrintErr($"WARN: DigitalClockObject.<init> : Failed to find required {finding} node.");
            return;
        }
        
        _timeMenu = new TimeDisplayMenu();
        _timeMenu.ModifyForm(form => form.SetCaptureInput(false));
        _timeMenu.DisplayOn(_viewport);
    }

    public override string GetDisplayName() => Items.DIGITAL_CLOCK.GetItemName();
    public override string GetContext() => $"Day: {EnvironmentManager.GetDay()+1}";
    public override string GetSummary() => "";

    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);

    public void Process(float delta) {
        if (GameUtils.IsNodeInvalid(GetBaseNode())) return;

        (int hours, int minutes) = EnvironmentManager.GetTimeAs24H();
        _timeMenu.GetForm().SetTime(hours, minutes);
        
        if (minutes == _lastMinute) return;
        _lastMinute = minutes;
        
        _timeMenu.GetForm().ShowDivider(_showDivider);
        _showDivider = !_showDivider;
    }

    public float GetSize() => 2.0f;
}