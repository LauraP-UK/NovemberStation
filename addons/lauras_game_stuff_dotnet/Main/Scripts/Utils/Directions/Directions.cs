using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Enum-like class for Directions.
/// </summary>
public static class Directions {

    public static readonly Direction SELF = Direction.Create("Self", Vector3.Zero, SimpleDirection.SELF);
    public static readonly Direction NORTH = Direction.Create("North", Vector3.Back, SimpleDirection.NORTH);
    public static readonly Direction EAST = Direction.Create("East", Vector3.Right, SimpleDirection.EAST);
    public static readonly Direction SOUTH = Direction.Create("South", Vector3.Forward, SimpleDirection.SOUTH);
    public static readonly Direction WEST = Direction.Create("West", Vector3.Left, SimpleDirection.WEST);
    public static readonly Direction UP = Direction.Create("Up", Vector3.Up, SimpleDirection.UP);
    public static readonly Direction DOWN = Direction.Create("Down", Vector3.Down, SimpleDirection.DOWN);
    public static readonly Direction NORTH_EAST = Direction.Create("North East", NORTH, EAST, SimpleDirection.NORTH_EAST);
    public static readonly Direction SOUTH_EAST = Direction.Create("South East", SOUTH, EAST, SimpleDirection.SOUTH_EAST);
    public static readonly Direction SOUTH_WEST = Direction.Create("South West", SOUTH, WEST, SimpleDirection.SOUTH_WEST);
    public static readonly Direction NORTH_WEST = Direction.Create("North West", NORTH, WEST, SimpleDirection.NORTH_WEST);
    public static readonly Direction UP_NORTH = Direction.Create("Up North", UP, NORTH, SimpleDirection.UP_NORTH);
    public static readonly Direction UP_EAST = Direction.Create("Up East", UP, EAST, SimpleDirection.UP_EAST);
    public static readonly Direction UP_SOUTH = Direction.Create("Up South", UP, SOUTH, SimpleDirection.UP_SOUTH);
    public static readonly Direction UP_WEST = Direction.Create("Up West", UP, WEST, SimpleDirection.UP_WEST);
    public static readonly Direction DOWN_NORTH = Direction.Create("Down North", DOWN, NORTH, SimpleDirection.DOWN_NORTH);
    public static readonly Direction DOWN_EAST = Direction.Create("Down East", DOWN, EAST, SimpleDirection.DOWN_EAST);
    public static readonly Direction DOWN_SOUTH = Direction.Create("Down South", DOWN, SOUTH, SimpleDirection.DOWN_SOUTH);
    public static readonly Direction DOWN_WEST = Direction.Create("Down West", DOWN, WEST, SimpleDirection.DOWN_WEST);
    public static readonly Direction UP_NORTH_EAST = Direction.Create("Up North East", UP_NORTH, EAST, SimpleDirection.UP_NORTH_EAST);
    public static readonly Direction UP_SOUTH_EAST = Direction.Create("Up South East", UP_SOUTH, EAST, SimpleDirection.UP_SOUTH_EAST);
    public static readonly Direction UP_SOUTH_WEST = Direction.Create("Up South West", UP_SOUTH, WEST, SimpleDirection.UP_SOUTH_WEST);
    public static readonly Direction UP_NORTH_WEST = Direction.Create("Up North West", UP_NORTH, WEST, SimpleDirection.UP_NORTH_WEST);
    public static readonly Direction DOWN_NORTH_EAST = Direction.Create("Down North East", DOWN_NORTH, EAST, SimpleDirection.DOWN_NORTH_EAST);
    public static readonly Direction DOWN_SOUTH_EAST = Direction.Create("Down South East", DOWN_SOUTH, EAST, SimpleDirection.DOWN_SOUTH_EAST);
    public static readonly Direction DOWN_SOUTH_WEST = Direction.Create("Down South West", DOWN_SOUTH, WEST, SimpleDirection.DOWN_SOUTH_WEST);
    public static readonly Direction DOWN_NORTH_WEST = Direction.Create("Down North West", DOWN_NORTH, WEST, SimpleDirection.DOWN_NORTH_WEST);

    public static IEnumerable<Direction> GetCardinal() {
        return new[] { NORTH, EAST, SOUTH, WEST };
    }
    
    public static IEnumerable<Direction> GetAdjacent() {
        return new[] { NORTH, EAST, SOUTH, WEST, UP, DOWN };
    }

    public static IEnumerable<Direction> GetDiagonals() {
        return new[] { NORTH_EAST, SOUTH_EAST, SOUTH_WEST, NORTH_WEST };
    }

    public static IEnumerable<Direction> GetAll() {
        return new[] { SELF, NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORTH_WEST,
            UP, UP_NORTH, UP_NORTH_EAST, UP_EAST, UP_SOUTH_EAST, UP_SOUTH, UP_SOUTH_WEST, UP_WEST, UP_NORTH_WEST,
            DOWN, DOWN_NORTH, DOWN_NORTH_EAST, DOWN_EAST, DOWN_SOUTH_EAST, DOWN_SOUTH, DOWN_SOUTH_WEST, DOWN_WEST, DOWN_NORTH_WEST
        };
    }

    public static Direction GetOpposite(Direction direction) {
        return direction.SimpleDirection switch {
            SimpleDirection.SELF => SELF,
            SimpleDirection.NORTH => SOUTH,
            SimpleDirection.NORTH_EAST => SOUTH_WEST,
            SimpleDirection.EAST => WEST,
            SimpleDirection.SOUTH_EAST => NORTH_WEST,
            SimpleDirection.SOUTH => NORTH,
            SimpleDirection.SOUTH_WEST => NORTH_EAST,
            SimpleDirection.WEST => EAST,
            SimpleDirection.NORTH_WEST => SOUTH_EAST,
            SimpleDirection.UP => DOWN,
            SimpleDirection.DOWN => UP,
            SimpleDirection.UP_NORTH => DOWN_SOUTH,
            SimpleDirection.UP_EAST => DOWN_WEST,
            SimpleDirection.UP_SOUTH => DOWN_NORTH,
            SimpleDirection.UP_WEST => DOWN_EAST,
            SimpleDirection.DOWN_NORTH => UP_SOUTH,
            SimpleDirection.DOWN_EAST => UP_WEST,
            SimpleDirection.DOWN_SOUTH => UP_NORTH,
            SimpleDirection.DOWN_WEST => UP_EAST,
            SimpleDirection.UP_NORTH_EAST => DOWN_SOUTH_WEST,
            SimpleDirection.UP_SOUTH_EAST => DOWN_NORTH_WEST,
            SimpleDirection.UP_SOUTH_WEST => DOWN_NORTH_EAST,
            SimpleDirection.UP_NORTH_WEST => DOWN_SOUTH_EAST,
            SimpleDirection.DOWN_NORTH_EAST => UP_SOUTH_WEST,
            SimpleDirection.DOWN_SOUTH_EAST => UP_NORTH_WEST,
            SimpleDirection.DOWN_SOUTH_WEST => UP_NORTH_EAST,
            SimpleDirection.DOWN_NORTH_WEST => UP_SOUTH_EAST,
            _ => throw new ArgumentException("Invalid direction")
        };
    }

    public static Direction GetFromSimple(SimpleDirection simpleDirection) {
        return GetAll().FirstOrDefault(d => d.SimpleDirection == simpleDirection);
    }

    public static Direction GetFromName(string typeName) {
        return GetAll().FirstOrDefault(d => string.Equals(d.Name, typeName, StringComparison.OrdinalIgnoreCase));
    }
    
    public static Direction GetFromOffset(Vector3 offset) {
        return GetAll().FirstOrDefault(d => d.Offset == offset);
    }
}