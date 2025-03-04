
using Godot;

public class EnvironmentType {
    private readonly Color _ambientLightColor;
    private readonly float _ambientLightEnergy;
    private readonly bool _fogEnabled;
    private readonly Color _fogColor;
    private readonly float _fogDensity;
    private readonly Environment.ToneMapper _toneMapper;
    private readonly bool _glowEnabled;
    private readonly float _glowStrength;
    private readonly Sky _sky;
    
    private EnvironmentType(Color ambientLightColor, float ambientLightEnergy, bool fogEnabled, Color fogColor, float fogDensity,
        Environment.ToneMapper toneMapper, bool glowEnabled, float glowStrength, Sky sky) {
        _ambientLightColor = ambientLightColor;
        _ambientLightEnergy = ambientLightEnergy;
        _fogEnabled = fogEnabled;
        _fogColor = fogColor;
        _fogDensity = fogDensity;
        _toneMapper = toneMapper;
        _glowEnabled = glowEnabled;
        _glowStrength = glowStrength;
        _sky = sky;
    }
    
    public void Apply(WorldEnvironment worldEnv) {
        if (worldEnv == null || worldEnv.Environment == null) {
            GD.PrintErr("ERROR: EnvironmentType.Apply() : Invalid WorldEnvironment node.");
            return;
        }

        Environment env = worldEnv.Environment;
        env.AmbientLightColor = _ambientLightColor;
        env.AmbientLightEnergy = _ambientLightEnergy;
        env.FogEnabled = _fogEnabled;
        env.FogLightColor = _fogColor;
        env.FogDensity = _fogDensity;
        env.SetTonemapper(_toneMapper);
        env.GlowEnabled = _glowEnabled;
        env.GlowStrength = _glowStrength;
        env.Sky = _sky;
    }

    public override string ToString() => $"[EnvironmentType] AmbientLight={_ambientLightColor}, Fog={_fogEnabled}({_fogDensity}), Glow={_glowEnabled}({_glowStrength})";


    public static EnvironmentType Create(Color ambientLightColor, float ambientLightEnergy, bool fogEnabled, Color fogColor,
        float fogDensity, Environment.ToneMapper toneMapper, bool glowEnabled, float glowStrength, Sky sky) {
        return new EnvironmentType(ambientLightColor, ambientLightEnergy, fogEnabled, fogColor, fogDensity, toneMapper, glowEnabled, glowStrength, sky);
    }
}