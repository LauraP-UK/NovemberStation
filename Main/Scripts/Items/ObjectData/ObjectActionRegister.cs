
public static class ObjectActionRegister {
    private static readonly SmartDictionary<string, ObjectData> _registry = new();
    public static void Register(ObjectData data) => _registry.Add(data.GetMetaTag(), data);
    public static ObjectData Get(string key) => _registry.GetOrDefault(key, null);

    public static void Init() {
        new CubeObjectActions();
        new GasCanObjectActions();
        new DeskObjectActions();
        new PCObjectActions();
    } 
}