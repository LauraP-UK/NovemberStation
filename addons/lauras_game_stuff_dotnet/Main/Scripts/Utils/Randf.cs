using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Randf - Random Functions
/// <br/><br/>
/// Contains functions for generating and managing random numbers, and selecting / shuffling random elements from collections, sets, and maps.
/// </summary>
public static class Randf {
    /// <summary>
    /// Returns a value which is randomly selected between '<paramref name="min"/>' and '<paramref name="max"/>', with a bias to either smaller or larger numbers.
    /// The '<paramref name="bias"/>' argument is generally between 0.0 and 2.0, and will favour larger numbers when less than 1 and smaller numbers when greater than 1.
    /// <br/><br/>
    /// '<paramref name="max"/>' is exclusive for doubles and floats.
    /// </summary>
    /// <param name="min">The minimum of the range (inclusive).</param>
    /// <param name="max">The maximum of the range (exclusive for doubles and floats).</param>
    /// <param name="bias">The bias to favour smaller or larger numbers (greater than 1 = Smaller, less than 1 = Larger).</param>
    /// <returns>A randomly selected value between <paramref name="min"/> and <paramref name="max"/>.</returns>
    public static double RandomBias(double min, double max, double bias) {
        return (max - min) * Math.Pow(new Random().NextDouble(), bias) + min;
    }

    /// <summary>
    /// Returns a value which is randomly selected between '<paramref name="min"/>' and '<paramref name="max"/>', with a bias to either smaller or larger numbers.
    /// The '<paramref name="bias"/>' argument is generally between 0.0 and 2.0, and will favour larger numbers when less than 1 and smaller numbers when greater than 1.
    /// <br/><br/>
    /// '<paramref name="max"/>' is exclusive for doubles and floats.
    /// </summary>
    /// <param name="min">The minimum of the range (inclusive).</param>
    /// <param name="max">The maximum of the range (exclusive for doubles and floats).</param>
    /// <param name="bias">The bias to favour smaller or larger numbers (greater than 1 = Smaller, less than 1 = Larger).</param>
    /// <returns>A randomly selected value between <paramref name="min"/> and <paramref name="max"/>.</returns>
    public static float RandomBias(float min, float max, float bias) {
        return (float)((max - min) * Math.Pow(new Random().NextDouble(), bias) + min);
    }

    /// <summary>
    /// Returns a value which is randomly selected between '<paramref name="min"/>' and '<paramref name="max"/>', with a bias to either smaller or larger numbers.
    /// The '<paramref name="bias"/>' argument is generally between 0.0 and 2.0, and will favour larger numbers when less than 1 and smaller numbers when greater than 1.
    /// <br/><br/>
    /// '<paramref name="max"/>' is inclusive for integers and longs.
    /// </summary>
    /// <param name="min">The minimum of the range (inclusive).</param>
    /// <param name="max">The maximum of the range (exclusive for doubles and floats).</param>
    /// <param name="bias">The bias to favour smaller or larger numbers (greater than 1 = Smaller, less than 1 = Larger).</param>
    /// <returns>A randomly selected value between <paramref name="min"/> and <paramref name="max"/>.</returns>
    public static int RandomBias(int min, int max, double bias) {
        return (int)Math.Floor((max + 1 - min) * Math.Pow(new Random().NextDouble(), bias) + min);
    }

    /// <summary>
    /// Returns a value which is randomly selected between '<paramref name="min"/>' and '<paramref name="max"/>', with a bias to either smaller or larger numbers.
    /// The '<paramref name="bias"/>' argument is generally between 0.0 and 2.0, and will favour larger numbers when less than 1 and smaller numbers when greater than 1.
    /// <br/><br/>
    /// '<paramref name="max"/>' is inclusive for integers and longs.
    /// </summary>
    /// <param name="min">The minimum of the range (inclusive).</param>
    /// <param name="max">The maximum of the range (exclusive for doubles and floats).</param>
    /// <param name="bias">The bias to favour smaller or larger numbers (greater than 1 = Smaller, less than 1 = Larger).</param>
    /// <returns>A randomly selected value between <paramref name="min"/> and <paramref name="max"/>.</returns>
    public static long RandomBias(long min, long max, double bias) {
        return (long)Math.Floor((max + 1L - min) * Math.Pow(new Random().NextDouble(), bias) + min);
    }

    /// <summary>
    /// Returns a value which is randomly selected between '<paramref name="min"/>' and '<paramref name="max"/>', with a bias to either smaller or larger numbers.
    /// The '<paramref name="bias"/>' argument is generally between 0.0 and 2.0, and will favour larger numbers when less than 1 and smaller numbers when greater than 1.
    /// <br/><br/>
    /// '<paramref name="max"/>' is inclusive for integers and longs.
    /// </summary>
    /// <param name="min">The minimum of the range (inclusive).</param>
    /// <param name="max">The maximum of the range (exclusive for doubles and floats).</param>
    /// <param name="bias">The bias to favour smaller or larger numbers (greater than 1 = Smaller, less than 1 = Larger).</param>
    /// <returns>A randomly selected value between <paramref name="min"/> and <paramref name="max"/>.</returns>
    public static int RandomBias(int min, int max, float bias) {
        return (int)Math.Floor((max + 1 - min) * Math.Pow(new Random().NextDouble(), bias) + min);
    }

    /// <summary>
    /// Returns a value which is randomly selected between '<paramref name="min"/>' and '<paramref name="max"/>', with a bias to either smaller or larger numbers.
    /// The '<paramref name="bias"/>' argument is generally between 0.0 and 2.0, and will favour larger numbers when less than 1 and smaller numbers when greater than 1.
    /// <br/><br/>
    /// '<paramref name="max"/>' is inclusive for integers and longs.
    /// </summary>
    /// <param name="min">The minimum of the range (inclusive).</param>
    /// <param name="max">The maximum of the range (exclusive for doubles and floats).</param>
    /// <param name="bias">The bias to favour smaller or larger numbers (greater than 1 = Smaller, less than 1 = Larger).</param>
    /// <returns>A randomly selected value between <paramref name="min"/> and <paramref name="max"/>.</returns>
    public static long RandomBias(long min, long max, float bias) {
        return (long)Math.Floor((max + 1L - min) * Math.Pow(new Random().NextDouble(), bias) + min);
    }

    /// <summary>
    /// Returns a value which is randomly selected between <paramref name="min"/> and <paramref name="max"/>.
    /// <br/>
    /// <paramref name="max"/> is exclusive for doubles and floats.
    /// </summary>
    public static double Random(double min, double max) {
        return RandomBias(min, max, 1.0d);
    }
    
    /// <summary>
    /// Returns a value which is randomly selected between <paramref name="min"/> and <paramref name="max"/>.
    /// <br/>
    /// <paramref name="max"/> is exclusive for doubles and floats.
    /// </summary>
    public static float Random(float min, float max) {
        return RandomBias(min, max, 1.0f);
    }
    
    /// <summary>
    /// Returns a value which is randomly selected between <paramref name="min"/> and <paramref name="max"/>.
    /// <br/>
    /// <paramref name="max"/> is inclusive for integers and longs.
    /// </summary>
    public static int Random(int min, int max) {
        return RandomBias(min, max, 1.0d);
    }
    
    /// <summary>
    /// Returns a value which is randomly selected between <paramref name="min"/> and <paramref name="max"/>.
    /// <br/>
    /// <paramref name="max"/> is inclusive for integers and longs.
    /// </summary>
    public static long Random(long min, long max) {
        return RandomBias(min, max, 1.0d);
    }

    /// <summary>
    /// Simulates a roll of a die and returns true if the result is less than or equal to the given chance.
    /// </summary>
    public static bool RandomChanceIn(double chance, double inValue) {
        return Random(1.0d, Math.Abs(inValue)) <= Math.Abs(chance);
    }

    /// <summary>
    /// Simulates a roll of a die and returns true if the result is less than or equal to the given chance.
    /// </summary>
    public static bool RandomChanceIn(float chance, float inValue) {
        return Random(1.0f, Math.Abs(inValue)) <= Math.Abs(chance);
    }

    /// <summary>
    /// Simulates a roll of a die and returns true if the result is less than or equal to the given chance.
    /// </summary>
    public static bool RandomChanceIn(int chance, int inValue) {
        return Random(1, Math.Abs(inValue)) <= Math.Abs(chance);
    }

    /// <summary>
    /// Simulates a roll of a die and returns true if the result is less than or equal to the given chance.
    /// </summary>
    public static bool RandomChanceIn(long chance, long inValue) {
        return Random(1L, Math.Abs(inValue)) <= Math.Abs(chance);
    }

    /// <summary>
    /// Flips a coin for a 50/50 chance of returning true or false.
    /// </summary>
    public static bool Random() {
        return Random(0, 2) == 0;
    }

    /// <summary>
    /// Picks a random element from an array.
    /// </summary>
    public static T Random<T>(params T[] array) {
        if (array == null || array.Length == 0) return default;
        return array.Length == 1 ? array[0] : array[Random(0, array.Length - 1)];
    }

    /// <summary>
    /// Picks a random element from a collection.
    /// </summary>
    public static T Random<T>(IEnumerable<T> collection) {
        IEnumerable<T> enumerable = collection as T[] ?? collection.ToArray();
        if (!enumerable.Any()) return default;
        List<T> list = enumerable.ToList();
        return list[Random(0, list.Count - 1)];
    }

    /// <summary>
    /// Picks a random entry from a dictionary.
    /// </summary>
    public static KeyValuePair<K, V> Random<K, V>(IDictionary<K, V> dictionary) {
        if (dictionary == null || dictionary.Count == 0) return default;
        return Random<KeyValuePair<K, V>>(dictionary.ToList());
    }

    /// <summary>
    /// Creates a shuffled copy of a list.
    /// </summary>
    public static List<T> CopyAndShuffleList<T>(IEnumerable<T> collection) {
        IEnumerable<T> enumerable = collection as T[] ?? collection.ToArray();
        if (!enumerable.Any()) return new List<T>();
        List<T> list = enumerable.ToList();
        Random rng = new();
        return list.OrderBy(_ => rng.Next()).ToList();
    }
}