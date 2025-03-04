
using System.Collections.Generic;
using Godot;

public static class Environments {
    public static readonly EnvironmentType MORNING = EnvironmentType.Create(
        new Color(0.9f, 0.85f, 0.75f),   // Soft golden light
        1.2f,                            // Gentle ambient brightness
        true,                             // Light fog enabled
        new Color(0.85f, 0.85f, 0.9f),    // Cool misty morning fog
        0.015f,                           // Very light fog density
        Environment.ToneMapper.Linear,    // Natural tone mapping
        false,                            // No glow
        0.0f,
        new Sky()             // Default sky
    );

    public static readonly EnvironmentType DAY = EnvironmentType.Create(
        new Color(1.0f, 1.0f, 1.0f),      // Pure neutral white light
        1f,                             // Standard brightness
        false,                             // No fog needed
        new Color(0, 0, 0),               // No fog color
        0.0f,                              // No fog density
        Environment.ToneMapper.Linear,    // True-to-life tone mapping
        false,                            // No artificial glow
        0.0f,                           // No glow strength
        new Sky()             // Default sky
    );

    public static readonly EnvironmentType EVENING = EnvironmentType.Create(
        new Color(1.0f, 0.75f, 0.5f),     // Warm golden light
        1.1f,                             // Slightly dimmed brightness
        true,                             // Soft evening haze
        new Color(0.9f, 0.7f, 0.5f),      // Warm, gentle fog
        0.02f,                            // Soft atmospheric haze
        Environment.ToneMapper.Linear,    // Balanced tone mapping
        true,                             // Gentle glow from sunset
        0.2f,                            // Slight glow effect
        new Sky()             // Default sky
    );

    public static readonly EnvironmentType NIGHT = EnvironmentType.Create(
        new Color(0.3f, 0.35f, 0.5f),    // Subtle blue moonlight
        0.7f,                            // Soft ambient brightness (not too dark)
        true,                             // Subtle night fog
        new Color(0.2f, 0.25f, 0.4f),    // Deep blue atmospheric haze
        0.025f,                           // Slightly denser fog at night
        Environment.ToneMapper.Linear, // Cinematic contrast but natural
        true,                             // Soft glow for stars and distant lights
        0.25f,                           // Glow strength for night lights
        new Sky()             // Default sky
    );

    private static readonly EnvironmentType[] ALL_ENVIRONMENTS = { MORNING, DAY, EVENING, NIGHT };

    public static List<EnvironmentType> GetAll() => new(ALL_ENVIRONMENTS);
    
    public static EnvironmentType GetNext(EnvironmentType current) {
        int index = System.Array.IndexOf(ALL_ENVIRONMENTS, current);
        return ALL_ENVIRONMENTS[(index + 1) % ALL_ENVIRONMENTS.Length];
    }
}