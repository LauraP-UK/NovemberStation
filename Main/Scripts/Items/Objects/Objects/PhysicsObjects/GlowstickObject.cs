using System;
using Godot;

public class GlowstickObject : ObjectBase<RigidBody3D>, IGrabbable, ICollectable, IProcess, IVolumetricObject {
    private readonly OmniLight3D _light;
    private readonly MeshInstance3D _body;

    private const string OMNI_LIGHT_PATH = "Light", BODY_PATH = "StickBody";

    public const string LIGHT_COLOUR = "light_colour";
    [SerialiseData(LIGHT_COLOUR, nameof(SetColour))]
    private Color _colour = Colors.White;
    
    public const string PARTY_MODE = "party_mode";
    [SerialiseData(PARTY_MODE, nameof(SetPartyMode))]
    private bool _isPartyMode;

    private const float PARTY_LOOP_DURATION = 2.0f;
    private float _partyTimer;
    
    private readonly Color[] _colours = [Colors.White, Colors.Red, Colors.Aqua, Colors.Green, Colors.Yellow, Colors.HotPink];

    public GlowstickObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "glowstick_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_, _) => true, Grab);
        RegisterAction<ICollectable>((_, _) => GameManager.IsDebugMode(), Collect);
        RegisterArbitraryAction("Cycle Colour", 5, (_, _) => true, (_, ev) => {
            if (ev is not KeyPressEvent || _isPartyMode) return;
            SetColour(GetNextColour()); 
        });
        RegisterArbitraryAction("PARTY MODE", 6, (_, _) => GameManager.IsDebugMode(), (_, ev) => {
            if (ev is not KeyPressEvent) return;
            SetPartyMode(!_isPartyMode);
            if (!_isPartyMode) SetColour(_colours[0]);
        });

        _light = FindNode<OmniLight3D>(OMNI_LIGHT_PATH);
        _body = FindNode<MeshInstance3D>(BODY_PATH);

        SetColour();
    }

    public void SetColour(Color color = default) {
        if (color == default) color = Colors.White;
        _colour = color;
        _light.SetColor(color);

        _body.MaterialOverride = new StandardMaterial3D {
            AlbedoColor = color,
            MetallicSpecular = 0.0f,
            Metallic = 0.0f,
            EmissionEnabled = true,
            Emission = color,
            EmissionEnergyMultiplier = 2.5f,
            RimEnabled = true,
            Rim = 1.0f
        };
    }

    private Color GetNextColour() {
        int index = Array.IndexOf(_colours, GetColour());
        if (index == -1) index = 0;
        return _colours[(index + 1) % _colours.Length];
    }
    public void SetRandomColour() => SetColour(Randf.Random(_colours));

    public void SetPartyMode(bool partyMode = false) => _isPartyMode = partyMode;

    public Color GetColour() => _colour;
    public override string GetDisplayName() => Items.GLOWSTICK.GetItemName();
    public override string GetContext() => "";
    public override string GetSummary() => "";
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);
    public float GetSize() => 0.25f;

    public void Process(float delta) {
        if (GameUtils.IsNodeInvalid(GetBaseNode())) return;
        if (!_isPartyMode) return;
        _partyTimer += delta;
        if (_partyTimer > PARTY_LOOP_DURATION) _partyTimer = 0.0f;

        float ratio = Mathsf.Remap(0.0f, PARTY_LOOP_DURATION, _partyTimer, 0.0f, 1.0f);
        SetColour(ColourHelper.CycleRainbow(ratio));
    }
}