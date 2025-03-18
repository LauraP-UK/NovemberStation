using System;

public static class FilterHelper {
    public static Func<object, bool> MakeItemTypeFilter(ItemType itemType) {
        return obj => {
            if (obj is not string json) return false;
            return itemType.GetTypeID() == Serialiser.GetSpecificData<string>(Serialiser.ObjectSaveData.TYPE_ID, json);
        };
    }
}