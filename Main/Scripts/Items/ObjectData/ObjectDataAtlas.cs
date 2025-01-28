
using System;

public static class ObjectDataAtlas {
    private static readonly SmartDictionary<string, ObjectData> _registry = new();
    public static ObjectData Register(string metaTag, Action<ObjectData> onInit) {
        ObjectData data = new(metaTag, onInit);
        _registry.Add(data.GetMetaTag(), data);
        return data;
    }

    public static ObjectData Get(string metaTag) => _registry.GetOrDefault(metaTag, null);

    public static readonly ObjectData CUBE_ACTIONS = Register("cube_obj", data => {
        data.AddAction(ObjectActions.GRAB_ACTION);
        data.AddAction(ObjectActions.SHOVE_ACTION);
    });
    public static readonly ObjectData GAS_CAN_ACTIONS = Register("gascan_obj", data => {
        data.AddAction(ObjectActions.GRAB_ACTION);
        data.AddAction(ObjectActions.SHOVE_ACTION);
        data.AddAction(ObjectActions.DRINK_ACTION);
    });
    public static readonly ObjectData DESK_ACTIONS = Register("desk_obj", data => {
        data.AddAction(ObjectActions.GRAB_ACTION);
    });
    public static readonly ObjectData PC_ACTIONS = Register("pc_obj", data => {
        data.AddAction(ObjectActions.USE_ACTION);
    });
    
    private static ObjectData[] _allActions = {
        CUBE_ACTIONS,
        GAS_CAN_ACTIONS,
        DESK_ACTIONS,
        PC_ACTIONS
    };
}