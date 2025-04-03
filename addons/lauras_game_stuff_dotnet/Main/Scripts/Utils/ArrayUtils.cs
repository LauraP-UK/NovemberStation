using System;
using System.Collections.Generic;
using System.Linq;

public static class ArrayUtils {
    public static bool ExactMatch<T>(T[] array1, T[] array2) {
        if (array1.Length != array2.Length) return false;
        return !array1.Where((t, i) => !t.Equals(array2[i])).Any();
    }

    public static bool ExactMatch<T>(IEnumerable<T> array1, IEnumerable<T> array2) => ExactMatch(array1.ToArray(), array2.ToArray());

    public static bool ExactMatch<T1, T2>(IEnumerable<T1> array1, IEnumerable<T2> array2) {
        if (!typeof(T1).IsAssignableFrom(typeof(T2)) && !typeof(T2).IsAssignableFrom(typeof(T1))) return false;
        IEnumerable<T1> castedArray2 = array2.Cast<T1>();
        return ExactMatch(array1.ToArray(), castedArray2.ToArray());
    }

    public static bool ExactMatchSorted<T1, T2>(Func<KeyValuePair<T1, T2>, KeyValuePair<T1, T2>, int> sorter, IDictionary<T1, T2> dict1, IDictionary<T1, T2> dict2) {
        if (dict1.Count != dict2.Count) return false;

        List<KeyValuePair<T1, T2>> sorted1 = dict1.ToList();
        List<KeyValuePair<T1, T2>> sorted2 = dict2.ToList();

        sorted1.Sort(new Comparison<KeyValuePair<T1, T2>>(sorter));
        sorted2.Sort(new Comparison<KeyValuePair<T1, T2>>(sorter));
        
        for (int i = 0; i < sorted1.Count; i++) {
            if (!EqualityComparer<T1>.Default.Equals(sorted1[i].Key, sorted2[i].Key) ||
                !EqualityComparer<T2>.Default.Equals(sorted1[i].Value, sorted2[i].Value)) {
                return false;
            }
        }

        return true;
    }

    public static string ToString<T>(IEnumerable<T> array, Func<T, string> converter) => $"[{string.Join(", ", array.Select(converter))}]";
}