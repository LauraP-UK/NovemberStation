using System;
using System.Collections.Generic;
using System.Linq;

public class WeightedSelection<T> {
    private readonly List<WeightedPair<T>> _selections = new();

    public void AddSelection(float weight, T value) {
        _selections.Add(WeightedPair<T>.Of(weight, value));
    }

    public T Select() {
        if (_selections.Count == 0) throw new InvalidOperationException("ERROR: WeightedSelection.Select() : No selections available for choosing.");

        float totalWeight = _selections.Sum(weightedPair => weightedPair.Weight);
        float selectedWeight = Randf.Random(0, totalWeight);
        float cumulativeWeight = 0.0f;
        
        foreach (WeightedPair<T> weightedPair in _selections) {
            cumulativeWeight += weightedPair.Weight;
            if (selectedWeight <= cumulativeWeight) return weightedPair.Value;
        }

        throw new Exception("ERROR: WeightedSelection.Select() : Weighted selection failed. This should never happen.");
    }
}