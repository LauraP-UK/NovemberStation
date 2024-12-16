using System;
using System.Collections.Generic;

public class WeightedPair<T> {
    private readonly KeyValuePair<float, T> _pair;

    public float Weight => _pair.Key;
    public T Value => _pair.Value;

    private WeightedPair(float weight, T value) {
        if (weight <= 0) throw new ArgumentException("ERROR: WeightedPair.<init> : Weight must be greater than zero.", nameof(weight));

        _pair = new KeyValuePair<float, T>(weight, value);
    }

    public static WeightedPair<T> Of(float weight, T value) {
        return new WeightedPair<T>(weight, value);
    }

    public override string ToString() {
        return $"WeightedPair [Weight: {Weight}, Value: {Value}]";
    }
}