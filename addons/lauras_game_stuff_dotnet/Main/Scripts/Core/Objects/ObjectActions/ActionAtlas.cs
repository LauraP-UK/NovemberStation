
using System;
using System.Linq;

public static class ActionAtlas {
    public const string
        ERROR_NAME = "ERROR_NAME";
    
    private static readonly SmartDictionary<Type, (string name, int index)> _registry = new();

    static ActionAtlas() {
        Register<IGrabbable>("Grab", 0);
        Register<IShovable>("Shove", 1);
        Register<IDrinkable>("Drink", 2);
        Register<IUsable>("Use", 3);
    }
    
    private static void Register<T>(string actionName, int index) where T : IObjectAction => _registry.Add(typeof(T), (actionName, index));
    public static string GetActionName<T>() => GetActionData<T>().name;
    public static string GetActionName(Type actionType) => GetActionData(actionType).name;

    public static int GetActionIndex<T>() => GetActionData<T>().index;
    public static int GetActionIndex(Type actionType) => GetActionData(actionType).index;

    
    private static Type GetTypeFrom(IObjectAction action) => action.GetType().GetInterfaces().FirstOrDefault(i => _registry.ContainsKey(i));
    private static (string name, int index) GetActionData(Type type) => _registry.GetOrDefault(type, (ERROR_NAME, -1));
    private static (string name, int index) GetActionData<T>() => GetActionData(typeof(T));
    private static (string name, int index) GetActionData(IObjectAction action) => GetActionData(GetTypeFrom(action));
}