using System.Linq;
using Godot; // Assuming Godot's Vector2 is available

/// <summary>
/// Represents a Direction with various properties and methods.
/// Emulates Java Enums with extended functionality.
/// </summary>
public class DirectionInternal {
    public string Name { get; }
    public Vector2 Offset { get; }
    public SimpleDirection SimpleDirection { get; }

    private DirectionInternal(string name, Vector2 offset, SimpleDirection simpleDirection) {
        Name = name;
        Offset = offset;
        SimpleDirection = simpleDirection;
    }

    public Vector2 GetRelative(Vector2 coord) {
        return coord + Offset;
    }

    public DirectionInternal GetOpposite() {
        return Directions.GetOpposite(this);
    }

    public DirectionInternal GetClockwise(bool cardinalOnly = true) {
        return Directions.GetClockwise(this, cardinalOnly);
    }

    public DirectionInternal GetCounterClockwise(bool cardinalOnly = true) {
        return Directions.GetCounterClockwise(this, cardinalOnly);
    }

    public bool IsCardinal() {
        return Directions.GetCardinal().Contains(this);
    }

    public static DirectionInternal Create(string name, Vector2 offset, SimpleDirection simpleDirection) {
        return new DirectionInternal(name, offset, simpleDirection);
    }

    public static DirectionInternal Combine(string name, DirectionInternal directionA, DirectionInternal directionB, SimpleDirection simpleDirection) {
        return new DirectionInternal(name, directionA.Offset + directionB.Offset, simpleDirection);
    }
}