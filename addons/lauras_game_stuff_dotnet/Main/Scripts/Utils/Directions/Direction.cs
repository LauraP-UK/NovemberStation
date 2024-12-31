using System.Linq;
using Godot; // Assuming Godot's Vector2 is available

/// <summary>
/// Represents a Direction with various properties and methods.
/// Emulates Java Enums with extended functionality.
/// </summary>
public class Direction {
    public string Name { get; }
    public Vector3 Offset { get; }
    public SimpleDirection SimpleDirection { get; }

    private Direction(string name, Vector3 offset, SimpleDirection simpleDirection) {
        Name = name;
        Offset = offset;
        SimpleDirection = simpleDirection;
    }

    public Vector3 GetRelative(Vector3 coord) {
        return coord + Offset;
    }

    public Direction GetOpposite() {
        return Directions.GetOpposite(this);
    }

    public bool IsCardinal() {
        return Directions.GetCardinal().Contains(this);
    }

    public bool IsAdjacent() {
        return Directions.GetAdjacent().Contains(this);
    }
    
    public Color GetAxisDebugColor() {
        return SimpleDirection switch {
            SimpleDirection.UP => Colors.Green,
            SimpleDirection.DOWN => Colors.Green,
            SimpleDirection.WEST => Colors.Red,
            SimpleDirection.EAST => Colors.Red,
            SimpleDirection.NORTH => Colors.Blue,
            SimpleDirection.SOUTH => Colors.Blue,
            _ => Colors.White
        };
    }

    public static Direction Create(string name, Vector3 offset, SimpleDirection simpleDirection) {
        return new Direction(name, offset, simpleDirection);
    }

    public static Direction Create(string name, Direction directionA, Direction directionB, SimpleDirection simpleDirection) {
        return new Direction(name, directionA.Offset + directionB.Offset, simpleDirection);
    }
}