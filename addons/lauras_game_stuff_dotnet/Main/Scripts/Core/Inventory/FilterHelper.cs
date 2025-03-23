using System;
using System.Collections.Generic;

public static class FilterHelper {
    public static Func<object, bool> MakeItemTypeFilter(ItemType itemType) {
        return obj => {
            if (obj is not string json) return false;
            return itemType.GetTypeID() == Serialiser.GetSpecificTag<string>(Serialiser.ObjectSaveData.TYPE_ID, json);
        };
    }
    public static IEnumerable<Func<object, bool>> MakeItemTypeFilters(params ItemType[] itemTypes) {
        HashSet<Func<object, bool>> filters = new();
        foreach (ItemType itemType in itemTypes) filters.Add(MakeItemTypeFilter(itemType));
        return filters;
    }
}