using System.Collections.Generic;
using Godot;

public static class Environments {
    public static readonly EnvironmentType MORNING = EnvironmentType.Create("Morning",
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

    public static readonly EnvironmentType DAY = EnvironmentType.Create("Day",
        1.0f,
        new Color(0.0f, 0.0f, 0.0f),
        1.0f,
        4.0f,
        true,
        new Color(0.0f, 0.0f, 0.0f),
        0.0f,
        1.0f,
        0.0f,
        true,
        1.0f
    );

    public static readonly EnvironmentType EVENING = EnvironmentType.Create("Evening",
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

    public static readonly EnvironmentType NIGHT = EnvironmentType.Create("Night",
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

    private static readonly EnvironmentType[] ALL_ENVIRONMENTS = { MORNING, DAY, EVENING, NIGHT };

    public static List<EnvironmentType> GetAll() => new(ALL_ENVIRONMENTS);
    public static int GetCount() => ALL_ENVIRONMENTS.Length;

    public static EnvironmentType GetNext(EnvironmentType current) {
        int index = System.Array.IndexOf(ALL_ENVIRONMENTS, current);
        return ALL_ENVIRONMENTS[(index + 1) % ALL_ENVIRONMENTS.Length];
    }
}