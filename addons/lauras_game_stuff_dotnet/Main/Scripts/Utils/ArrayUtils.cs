
using System.Collections.Generic;
using System.Linq;

public static class ArrayUtils {
    public static bool ExactMatch<T>(T[] array1, T[] array2) {
        if (array1.Length != array2.Length)
            return false;
        return !array1.Where((t, i) => !t.Equals(array2[i])).Any();
    }
    public static bool ExactMatch<T>(IEnumerable<T> array1, IEnumerable<T> array2) => ExactMatch(array1.ToArray(), array2.ToArray());
    public static bool ExactMatch<T1, T2>(IEnumerable<T1> array1, IEnumerable<T2> array2) {
        if (!typeof(T1).IsAssignableFrom(typeof(T2)) && !typeof(T2).IsAssignableFrom(typeof(T1)))
            return false;
        IEnumerable<T1> castedArray2 = array2.Cast<T1>();
        return ExactMatch(array1.ToArray(), castedArray2.ToArray());
    }
}