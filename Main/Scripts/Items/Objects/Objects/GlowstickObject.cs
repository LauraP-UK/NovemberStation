
using Godot;

public class GlowstickObject : ObjectBase<RigidBody3D>, IGrabbable, ICollectable, IVolumetricObject {
    
    private readonly OmniLight3D _light;
    private readonly MeshInstance3D _body;
    
    private const string OMNI_LIGHT_PATH = "Light", BODY_PATH = "StickBody";
    
    public const string LIGHT_COLOUR = "light_colour";
    [SerialiseData(LIGHT_COLOUR, nameof(SetColour), nameof(SetColour))]
    private Color _colour = Colors.White;
    
    public GlowstickObject(RigidBody3D baseNode, bool dataOnly = false) : base(baseNode, "glowstick_obj") {
        if (dataOnly) return;
        RegisterAction<IGrabbable>((_,_) => true, Grab);
        RegisterAction<ICollectable>((_,_) => true, Collect);
        
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
    
    public Color GetColour() => _colour;

    public override string GetDisplayName() => Items.GLOWSTICK.GetItemName();
    public override string GetContext() => "";
    public override string GetSummary() => "";
    public void Grab(ActorBase actorBase, IEventBase ev) => GrabActionDefault.Invoke(actorBase, GetBaseNode(), ev);
    public void Collect(ActorBase actorBase, IEventBase ev) => CollectActionDefault.Invoke(actorBase, this, ev);

    public float GetSize() => 0.25f;
}