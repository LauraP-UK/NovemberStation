using Godot;

public static class Environments {
    
    /* --- MORNINGS --- */
    
    public static readonly EnvironmentType MORNING_CLEAR = EnvironmentType.Create("ClearMorning",
        0.2f,
        new Color(0.0f, 0.0f, 0.0f),
        0.5f,
        1.0f,
        true,
        new Color(0.85f, 0.75f, 0.6f),
        0.3f,
        0.5f,
        0.005f,
        false,
        1.0f
    );
    public static readonly EnvironmentType MORNING_HAZY = EnvironmentType.Create("HazyMorning",
        0.25f,
        new Color(0.05f, 0.05f, 0.05f),
        0.6f,
        1.2f,
        true,
        new Color(0.9f, 0.8f, 0.65f),
        0.35f,
        0.6f,
        0.007f,
        false,
        1.0f
    );
    public static readonly EnvironmentType MORNING_FOGGY = EnvironmentType.Create("FoggyMorning",
        0.15f,
        new Color(0.0f, 0.0f, 0.0f),
        0.4f,
        0.8f,
        true,
        new Color(0.7f, 0.75f, 0.8f),
        0.4f,
        0.8f,
        0.015f,
        false,
        1.0f
    );
    public static readonly EnvironmentType MORNING_CLOUDY = EnvironmentType.Create("CloudyMorning",
        0.2f,
        new Color(0.1f, 0.1f, 0.1f),
        0.45f,
        0.9f,
        true,
        new Color(0.85f, 0.75f, 0.7f),
        0.2f,
        0.4f,
        0.005f,
        false,
        1.0f
    );

    /* --- DAYS --- */
    
    public static readonly EnvironmentType DAY_CLEAR = EnvironmentType.Create("ClearDay",
        1.0f,
        new Color(0.8f, 0.8f, 0.8f),
        0.750f,
        0.750f,
        true,
        new Color(0.0f, 0.0f, 0.0f),
        0.0f,
        1.0f,
        0.0f,
        true,
        1.0f
    );
    public static readonly EnvironmentType DAY_OVERCAST = EnvironmentType.Create("OvercastDay",
        0.8f,
        new Color(0.1f, 0.1f, 0.1f),
        0.75f,
        3.0f,
        true,
        new Color(0.5f, 0.5f, 0.6f),
        0.1f,
        1.0f,
        0.008f,
        true,
        1.0f
    );
    public static readonly EnvironmentType DAY_HOT = EnvironmentType.Create("HotDay",
        1.2f,
        new Color(0.0f, 0.0f, 0.0f),
        1.0f,
        5.0f,
        true,
        new Color(0.0f, 0.0f, 0.0f),
        0.0f,
        1.0f,
        0.0f,
        true,
        1.0f
    );
    public static readonly EnvironmentType DAY_HAZY = EnvironmentType.Create("HazyDay",
        0.9f,
        new Color(0.1f, 0.1f, 0.1f),
        0.9f,
        0.5f,
        true,
        new Color(0.8f, 0.8f, 0.8f),
        0.5f,
        0.5f,
        0.025f,
        true,
        1.0f
    );

    /* --- EVENINGS --- */
    
    public static readonly EnvironmentType EVENING_CLEAR = EnvironmentType.Create("ClearEvening",
        1.0f,
        new Color(0.0f, 0.0f, 0.0f),
        0.5f,
        1.0f,
        true,
        new Color(0.9f, 0.7f, 0.5f),
        0.2f,
        0.5f,
        0.005f,
        true,
        1.0f
    );
    public static readonly EnvironmentType EVENING_HAZY = EnvironmentType.Create("HazyEvening",
        0.7f,
        new Color(0.05f, 0.03f, 0.02f),
        0.4f,
        0.8f,
        true,
        new Color(0.8f, 0.6f, 0.5f),
        0.3f,
        0.6f,
        0.007f,
        true,
        1.0f
    );
    public static readonly EnvironmentType EVENING_OVERCAST = EnvironmentType.Create("OvercastEvening",
        0.6f,
        new Color(0.1f, 0.1f, 0.1f),
        0.3f,
        0.6f,
        true,
        new Color(0.5f, 0.4f, 0.35f),
        0.15f,
        0.5f,
        0.01f,
        true,
        1.0f
    );
    public static readonly EnvironmentType EVENING_BLOOD_RED = EnvironmentType.Create("BloodRedEvening",
        0.5f,
        new Color(0.8f, 0.2f, 0.1f),
        0.6f,
        0.08f,
        true,
        new Color(0.1f, 0.04f, 0.03f),
        0.2f,
        0.4f,
        0.004f,
        true,
        1.0f
    );
    
    /* --- NIGHTS --- */

    public static readonly EnvironmentType NIGHT_PITCH_BLACK = EnvironmentType.Create("PitchBlackNight",
        0.1f,
        new Color(0.0f, 0.0f, 0.0f),
        0.05f,
        0.3f,
        true,
        new Color(0.0f, 0.0f, 0.0f),
        0.0f,
        0.0f,
        0.0f,
        true,
        1.0f
    );
    public static readonly EnvironmentType NIGHT_CLEAR = EnvironmentType.Create("ClearNight",
        1.0f,
        new Color(0.0f, 0.0f, 0.0f),
        0.1f,
        0.15f,
        true,
        new Color(0.0f, 0.0f, 0.0f),
        0.0f,
        0.01f,
        0.0f,
        true,
        1.0f
    );
    public static readonly EnvironmentType NIGHT_FOGGY = EnvironmentType.Create("FoggyNight",
        0.1f,
        new Color(0.0f, 0.0f, 0.0f),
        0.05f,
        0.5f,
        true,
        new Color(0.2f, 0.25f, 0.4f),
        0.05f,
        0.0f,
        0.025f,
        true,
        1.0f
    );
    public static readonly EnvironmentType NIGHT_STORMY = EnvironmentType.Create("StormyNight",
        0.05f,
        new Color(0.02f, 0.02f, 0.02f),
        0.02f,
        0.1f,
        true,
        new Color(0.15f, 0.15f, 0.2f),
        0.08f,
        0.1f,
        0.02f,
        true,
        1.0f
    );
    public static readonly EnvironmentType NIGHT_DENSE_FOG = EnvironmentType.Create("DenseFogNight",
        0.05f,
        new Color(0.0f, 0.0f, 0.0f),
        0.02f,
        0.05f,
        true,
        new Color(0.2f, 0.25f, 0.3f),
        0.05f,
        0.05f,
        0.13f,
        true,
        1.0f
    );
    
    public static EnvironmentType[] GetMorningTypes() => new[] { MORNING_CLEAR, MORNING_HAZY, MORNING_FOGGY, MORNING_CLOUDY };
    public static EnvironmentType[] GetDayTypes() => new[] { DAY_CLEAR, DAY_OVERCAST, DAY_HOT, DAY_HAZY };
    public static EnvironmentType[] GetEveningTypes() => new[] { EVENING_CLEAR, EVENING_HAZY, EVENING_OVERCAST, EVENING_BLOOD_RED };
    public static EnvironmentType[] GetNightTypes() => new[] { NIGHT_CLEAR, NIGHT_FOGGY, NIGHT_PITCH_BLACK, NIGHT_STORMY, NIGHT_DENSE_FOG };
}