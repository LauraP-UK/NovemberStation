using System;
using System.Linq;

public static class ActionAtlas {
    public const string
        ERROR_NAME = "ERROR_NAME";

    private static readonly SmartDictionary<Type, (string name, int index)> _registry = new();
    private static readonly SmartDictionary<ActionKey, (string name, int index)> _customRegistry = new();

    static ActionAtlas() {
        Register<IGrabbable>("Grab", 0);
        Register<IShovable>("Shove", 1);
        Register<ICollectable>("Collect", 2);
        Register<IUsable>("Use", 3);
        Register<IDrinkable>("Drink", 4);
    }

    private static void Register<T>(string actionName, int index) where T : IObjectAction => _registry.Add(typeof(T), (actionName, index));
    public static string GetActionName<T>() => GetActionData<T>().name;
    public static string GetActionName(Type actionType) => GetActionData(actionType).name;

    public static int GetActionIndex<T>() => GetActionData<T>().index;
    public static int GetActionIndex(Type actionType) => GetActionData(actionType).index;


    public static void RegisterCustom(string name, int index) => _customRegistry[new ActionKey(name)] = (name, index);

    public static string GetActionName(ActionKey key) => GetValue(key).name;
    public static int GetActionIndex(ActionKey key) => GetValue(key).index;
    private static (string name, int index) GetValue(ActionKey key) =>
        !key.IsCustom ? _registry.GetOrDefault(key.InterfaceType, ("", -1)) : _customRegistry.GetOrDefault(key, ("", -1));


    private static Type GetTypeFrom(IObjectAction action) => action.GetType().GetInterfaces().FirstOrDefault(i => _registry.ContainsKey(i));
    private static (string name, int index) GetActionData(Type type) => _registry.GetOrDefault(type, (ERROR_NAME, -1));
    private static (string name, int index) GetActionData<T>() => GetActionData(typeof(T));
    private static (string name, int index) GetActionData(IObjectAction action) => GetActionData(GetTypeFrom(action));
}