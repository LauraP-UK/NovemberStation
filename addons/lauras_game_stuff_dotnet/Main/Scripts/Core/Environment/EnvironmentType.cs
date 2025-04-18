using Godot;

public class EnvironmentType {
    private readonly string _name;
    
    private readonly Color _ambientLightColor;
    private readonly float _backgroundEnergyMult, _ambientLightEnergy, _skyContribution;
    private readonly bool _fogEnabled;
    private readonly Color _fogColor;
    private readonly float _fogDensity, _fogLightEnergy, _aerialPerspective;
    private readonly bool _glowEnabled;
    private readonly float _glowStrength;

    private EnvironmentType(string name, float backgroundEnergyMult, Color ambientLightColor, float skyContribution, float ambientLightEnergy, bool fogEnabled, Color fogColor,
        float fogLightEnergy, float aerialPerspective, float fogDensity, bool glowEnabled, float glowStrength) {
        _name = name;
        _backgroundEnergyMult = backgroundEnergyMult;
        _ambientLightColor = ambientLightColor;
        _skyContribution = skyContribution;
        _ambientLightEnergy = ambientLightEnergy;
        _fogEnabled = fogEnabled;
        _fogColor = fogColor;
        _fogLightEnergy = fogLightEnergy;
        _aerialPerspective = aerialPerspective;
        _fogDensity = fogDensity;
        _glowEnabled = glowEnabled;
        _glowStrength = glowStrength;
    }

    public void Apply(WorldEnvironment worldEnv, float weight = 1.0f) {
        if (worldEnv == null || worldEnv.Environment == null) {
            GD.PrintErr("ERROR: EnvironmentType.Apply() : Invalid WorldEnvironment node.");
            return;
        }

        Environment env = worldEnv.Environment;
        env.AmbientLightColor = _ambientLightColor * weight;
        env.AmbientLightEnergy = _ambientLightEnergy * weight;
        env.FogEnabled = _fogEnabled;
        env.FogLightColor = _fogColor * weight;
        env.FogLightEnergy = _fogLightEnergy * weight;
        env.FogAerialPerspective = _aerialPerspective;
        env.FogDensity = _fogDensity * weight;
        env.GlowEnabled = _glowEnabled;
        env.GlowStrength = _glowStrength * weight;
        env.BackgroundEnergyMultiplier = _backgroundEnergyMult;
        env.AmbientLightSkyContribution = _skyContribution;
    }

    public override string ToString() {
        return $"EnvironmentType({_name})  |  Background Energy Mult: {_backgroundEnergyMult}  |  Ambient Light Color: {_ambientLightColor}  |  Sky Contribution: {_skyContribution}  |  Ambient Light Energy: {_ambientLightEnergy}  |  Fog Enabled: {_fogEnabled}  |  Fog Color: {_fogColor}  |  Fog Light Energy: {_fogLightEnergy}  |  Aerial Perspective: {_aerialPerspective}  |  Fog Density: {_fogDensity}  |  Glow Enabled: {_glowEnabled}  |  Glow Strength: {_glowStrength}";
    }

    public static EnvironmentType Create(string name, float backgroundEnergyMult, Color ambientLightColor, float skyContribution, float ambientLightEnergy, bool fogEnabled, Color fogColor,
        float fogLightEnergy, float aerialPerspective, float fogDensity, bool glowEnabled, float glowStrength) {
        return new EnvironmentType(name, backgroundEnergyMult, ambientLightColor, skyContribution, ambientLightEnergy, fogEnabled, fogColor, fogLightEnergy, aerialPerspective, fogDensity, glowEnabled,
            glowStrength);
    }

    public EnvironmentType BlendWith(EnvironmentType other, float weight, EaseType ease = null) {
        weight = Mathsf.Clamp(0.0f, 1.0f, weight);
        ease ??= Easing.LINEAR;
        
        weight = ease.Ease(0.0f, 1.0f, weight);

        return new EnvironmentType($"Blended_{_name}_{other._name}",
            Mathsf.Lerp(_backgroundEnergyMult, other._backgroundEnergyMult, weight),
            _ambientLightColor.Lerp(other._ambientLightColor, weight),
            Mathsf.Lerp(_skyContribution, other._skyContribution, weight),
            Mathsf.Lerp(_ambientLightEnergy, other._ambientLightEnergy, weight),
            other._fogEnabled || _fogEnabled,
            _fogColor.Lerp(other._fogColor, weight),
            Mathsf.Lerp(_fogLightEnergy, other._fogLightEnergy, weight),
            Mathsf.Lerp(_aerialPerspective, other._aerialPerspective, weight),
            Mathsf.Lerp(_fogDensity, other._fogDensity, weight),
            other._glowEnabled || _glowEnabled,
            Mathsf.Lerp(_glowStrength, other._glowStrength, weight)
        );
    }
    public string GetName() => _name;
}