using System;
using System.Linq;

/// <summary>
/// Mathsf - Mathematical Functions
/// <br/><br/>
/// Contains common, uncommon, and slightly less than uncommon functions for mathematical operations, which are commonly useful for game mechanics.
/// </summary>
public static class Mathsf {
    /// <summary>
    /// Clamps a value to the specified minimum and maximum bounds.
    /// </summary>
    public static int Clamp(int min, int max, int value) => Math.Max(Math.Min(value, max), min);

    /// <summary>
    /// Clamps a value to the specified minimum and maximum bounds.
    /// </summary>
    public static double Clamp(double min, double max, double value) => Math.Max(Math.Min(value, max), min);

    /// <summary>
    /// Clamps a value to the specified minimum and maximum bounds.
    /// </summary>
    public static float Clamp(float min, float max, float value) => Math.Max(Math.Min(value, max), min);

    /// <summary>
    /// Clamps a value to the specified minimum and maximum bounds.
    /// </summary>
    public static long Clamp(long min, long max, long value) => Math.Max(Math.Min(value, max), min);

    /// <summary>
    /// Clamps a value to the specified minimum and maximum bounds.
    /// </summary>
    public static int Clamp(int min, int max, double value) => (int)Math.Max(Math.Min(value, max), min);

    /// <summary>
    /// Clamps a value to the specified minimum and maximum bounds.
    /// </summary>
    public static long Clamp(long min, long max, double value) => (long)Math.Max(Math.Min(value, max), min);

    /// <summary>
    /// Rounds the given value to a specified number of significant digits.
    /// </summary>
    public static double Round(double value, int significantDigits) {
        if (significantDigits < 0) throw new ArgumentException("Significant digits must be greater than or equal to 0.");
        double factor = Math.Pow(10, significantDigits);
        return Math.Round(value * factor) / factor;
    }
    
    /// <summary>
    /// Rounds the given value to a specified number of significant digits.
    /// </summary>
    public static float Round(float value, int significantDigits) {
        if (significantDigits < 0) throw new ArgumentException("Significant digits must be greater than or equal to 0.");
        float factor = (float)Math.Pow(10, significantDigits);
        return MathF.Round(value * factor) / factor;
    }
    
    /// <summary>
    /// Rounds the given value to the nearest given increment.
    /// </summary>
    public static float RoundTo(float value, float increment) {
        if (increment == 0) throw new ArgumentException("Increment must not be zero.");
        return (float)(Math.Round(value / increment) * increment);
    }
    
    /// <summary>
    /// Rounds the given value to the nearest given increment.
    /// </summary>
    public static double RoundTo(double value, float increment) {
        if (increment == 0) throw new ArgumentException("Increment must not be zero.");
        return Math.Round(value / increment) * increment;
    }

    /// <summary>
    /// Performs linear interpolation between two values by a specified ratio.
    /// </summary>
    public static double Lerp(double start, double end, double ratio) => start + (end - start) * ratio;

    /// <summary>
    /// Performs linear interpolation between two values by a specified ratio.
    /// </summary>
    public static float Lerp(float start, float end, float ratio) => start + (end - start) * ratio;
    
    /// <summary>
    /// Performs linear interpolation between two values by a specified ratio.
    /// </summary>
    public static int Lerp(int start, int end, double ratio) => (int)(start + (end - start) * ratio);

    /// <summary>
    /// Performs linear interpolation between two values by a specified ratio.
    /// </summary>
    public static long Lerp(long start, long end, double ratio) => (long)(start + (end - start) * ratio);

    /// <summary>
    /// Calculates the shortest angle difference between two angles.
    /// </summary>
    public static double DeltaAngle(double angleA, double angleB) {
        double delta = ((angleB - angleA + 180) % 360 + 360) % 360 - 180;
        return delta;
    }

    /// <summary>
    /// Calculates the shortest angle difference between two angles.
    /// </summary>
    public static float DeltaAngle(float angleA, float angleB) {
        float delta = ((angleB - angleA + 180) % 360 + 360) % 360 - 180;
        return delta;
    }

    /// <summary>
    /// Performs inverse linear interpolation between two values.
    /// </summary>
    public static double InverseLerp(double start, double end, double value) {
        if (start == end) throw new ArgumentException("ERROR: Mathsf.InverseLerp() : Start and end values must not be the same.");
        return (value - start) / (end - start);
    }

    /// <summary>
    /// Performs inverse linear interpolation between two values.
    /// </summary>
    public static float InverseLerp(float start, float end, float value) {
        if (start == end) throw new ArgumentException("ERROR: Mathsf.InverseLerp() : Start and end values must not be the same.");
        return (value - start) / (end - start);
    }

    /// <summary>
    /// Performs inverse linear interpolation between two values, result is clamped between 0 and 1.
    /// </summary>
    public static double InverseLerpClamped(double start, double end, double value) {
        if (start == end) throw new ArgumentException("ERROR: Mathsf.InverseLerpClamped() : Start and end values must not be the same.");
        return Math.Clamp(InverseLerp(start, end, value), 0.0D, 1.0D);
    }

    /// <summary>
    /// Performs inverse linear interpolation between two values, result is clamped between 0 and 1.
    /// </summary>
    public static float InverseLerpClamped(float start, float end, float value) {
        if (start == end) throw new ArgumentException("ERROR: Mathsf.InverseLerpClamped() : Start and end values must not be the same.");
        return Math.Clamp(InverseLerp(start, end, value), 0.0f, 1.0f);
    }

    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    public static double Remap(double fromMin, double fromMax, double value, double toMin, double toMax) {
        double ratio = InverseLerp(fromMin, fromMax, value);
        return Lerp(toMin, toMax, ratio);
    }

    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    public static float Remap(float fromMin, float fromMax, float value, float toMin, float toMax) {
        float ratio = InverseLerp(fromMin, fromMax, value);
        return Lerp(toMin, toMax, ratio);
    }

    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    public static int Remap(int fromMin, int fromMax, int value, int toMin, int toMax) {
        double ratio = InverseLerp(fromMin, fromMax, value);
        return Lerp(toMin, toMax, ratio);
    }

    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    public static long Remap(long fromMin, long fromMax, long value, long toMin, long toMax) {
        double ratio = InverseLerp(fromMin, fromMax, value);
        return Lerp(toMin, toMax, ratio);
    }

    /// <summary>
    /// Returns the value closest to a target value from an array of values.
    /// </summary>
    public static T ClosestTo<T>(T target, params T[] values) where T : IComparable<T> {
        if (values == null || values.Length == 0) throw new ArgumentException("Values array must not be null or empty.");
        return values.OrderBy(v => Math.Abs((dynamic)v - (dynamic)target)).First();
    }
    
    /// <summary>
    /// Gets the max value from an array of values.
    /// </summary>
    public static T Max<T>(params T[] values) where T : IComparable<T> {
        if (values == null || values.Length == 0) throw new ArgumentException("Values array must not be null or empty.");
        return values.Max();
    }
    
    /// <summary>
    /// Gets the min value from an array of values.
    /// </summary>
    public static T Min<T>(params T[] values) where T : IComparable<T> {
        if (values == null || values.Length == 0) throw new ArgumentException("Values array must not be null or empty.");
        return values.Min();
    }

    /// <summary>
    /// Computes a quadratic equation given coefficients a, b, and c.
    /// </summary>
    public static double Quadratic(double a, double b, double c, double x) => a * Math.Pow(x, 2) + b * x + c;

    /// <summary>
    /// Computes a normalized quadratic equation.
    /// </summary>
    public static double Quadratic(double x) => Math.Pow(x, 2);
}