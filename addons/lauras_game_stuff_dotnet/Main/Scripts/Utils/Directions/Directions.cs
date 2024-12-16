using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// Enum-like class for Directions.
/// </summary>
public static class Directions {

    public static readonly DirectionInternal SELF = DirectionInternal.Create("Self", new Vector2(0, 0), SimpleDirection.SELF);

    public static readonly DirectionInternal NORTH = DirectionInternal.Create("North", new Vector2(0, -1), SimpleDirection.NORTH);

    public static readonly DirectionInternal EAST = DirectionInternal.Create("East", new Vector2(1, 0), SimpleDirection.EAST);

    public static readonly DirectionInternal SOUTH = DirectionInternal.Create("South", new Vector2(0, 1), SimpleDirection.SOUTH);

    public static readonly DirectionInternal WEST = DirectionInternal.Create("West", new Vector2(-1, 0), SimpleDirection.WEST);

    public static readonly DirectionInternal NORTH_EAST = DirectionInternal.Combine("North East", NORTH, EAST, SimpleDirection.NORTH_EAST);

    public static readonly DirectionInternal SOUTH_EAST = DirectionInternal.Combine("South East", SOUTH, EAST, SimpleDirection.SOUTH_EAST);

    public static readonly DirectionInternal SOUTH_WEST = DirectionInternal.Combine("South West", SOUTH, WEST, SimpleDirection.SOUTH_WEST);

    public static readonly DirectionInternal NORTH_WEST = DirectionInternal.Combine("North West", NORTH, WEST, SimpleDirection.NORTH_WEST);

    public static IEnumerable<DirectionInternal> GetCardinal() {
        return new[] { NORTH, EAST, SOUTH, WEST };
    }

    public static IEnumerable<DirectionInternal> GetDiagonals() {
        return new[] { NORTH_EAST, SOUTH_EAST, SOUTH_WEST, NORTH_WEST };
    }

    public static IEnumerable<DirectionInternal> GetAll() {
        return new[] { SELF, NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORTH_WEST };
    }

    public static DirectionInternal GetOpposite(DirectionInternal direction) {
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
            _ => throw new ArgumentException("Invalid direction")
        };
    }

    public static DirectionInternal GetClockwise(DirectionInternal direction, bool cardinalOnly = true) {
        IEnumerable<DirectionInternal> directions = cardinalOnly ? GetCardinal() : GetAll();
        List<DirectionInternal> list = directions.Where(d => d != SELF).ToList();
        int index = (list.IndexOf(direction) + 1) % list.Count;
        return list[index];
    }

    public static DirectionInternal GetCounterClockwise(DirectionInternal direction, bool cardinalOnly = true) {
        IEnumerable<DirectionInternal> directions = cardinalOnly ? GetCardinal() : GetAll();
        List<DirectionInternal> list = directions.Where(d => d != SELF).Reverse().ToList();
        int index = (list.IndexOf(direction) + 1) % list.Count;
        return list[index];
    }

    public static DirectionInternal GetFromSimple(SimpleDirection simpleDirection) {
        return GetAll().FirstOrDefault(d => d.SimpleDirection == simpleDirection);
    }

    public static DirectionInternal GetFromName(string typeName) {
        return GetAll().FirstOrDefault(d => string.Equals(d.Name, typeName, StringComparison.OrdinalIgnoreCase));
    }
}