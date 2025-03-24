using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Godot;

public static class ObjectAtlas {
    private static readonly SmartDictionary<string, Type> _registry = new();
    private static readonly SmartDictionary<Type, IObjectBase> _dataOnlyCache = new();
    private static readonly SmartDictionary<Type, SmartDictionary<string, SmartSerialData>> _serialiseDataCache = new();
    public const string OBJECT_TAG = "object_tag";
    static ObjectAtlas() => RegisterAll();

    private static void RegisterAll() {
        // Automatically registers all classes that inherit from ObjectBase<>
        GD.Print("INFO: ObjectAtlas.RegisterAll() : STARTUP. Registering all object classes...");
        IEnumerable<Type> objectTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IObjectBase).IsAssignableFrom(t));

        foreach (Type type in objectTypes) Register(type);
    }

    public static void Register([NotNull] Type clazz) {
        ArgumentNullException.ThrowIfNull(clazz);
        if (!typeof(IObjectBase).IsAssignableFrom(clazz)) throw new Exception($"ERROR: ObjectAtlas.Register() : Class '{clazz.Name}' must inherit from ObjectBase<?>.");

        GD.Print($"INFO: ObjectAtlas.Register() : Registering object class: {clazz.Name}...");

        try {
            object dummy = CreateObject(clazz, null);
            if (dummy is not IObjectBase obj)
                throw new Exception($"ERROR: ObjectAtlas.Register() : Created object is not of expected type 'IObjectBase'. Got '{dummy?.GetType()}'.");

            _registry.Add(obj.GetObjectTag(), clazz);

            if (_serialiseDataCache.ContainsKey(clazz)) return;
            RegisterSerialiseData(clazz, dummy);
        } catch (Exception e) {
            GD.PrintErr($"ERROR: ObjectAtlas.Register() : Failed to create instance of class '{clazz.Name}'. Exception: {e.Message}");
        }
    }

    public static SmartDictionary<string, SmartSerialData> GetSerialiseData(Type clazz) => _serialiseDataCache.GetOrDefault(clazz, null);

    private static void RegisterSerialiseData(Type clazz, object dummyObj) {
        FieldInfo[] fields = clazz.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        SmartDictionary<string, SmartSerialData> serialiseData = new();

        foreach (FieldInfo field in fields) {
            SerialiseDataAttribute attr = field.GetCustomAttribute<SerialiseDataAttribute>();
            if (attr == null) continue;
            if (serialiseData.ContainsKey(attr.Key)) {
                GD.PrintErr($"WARN: Serialiser.RegisterSerialiseData() : Duplicate key '{attr.Key}' for object '{clazz.Name}'.");
                continue;
            }

            if (attr.Handler == SerialiseDataAttribute.SerialiseHandler.INVENTORY || typeof(IInventory).IsAssignableFrom(field.FieldType)) {
                if (field.GetValue(dummyObj) is not IInventory) {
                    GD.PrintErr($"WARN: Serialiser.RegisterSerialiseData() : Expected IInventory but got {field.FieldType}.");
                    continue;
                }

                serialiseData[attr.Key] = SmartSerialData.FromInventory();
                continue;
            }

            MethodInfo setterMethod = clazz.GetMethod(attr.Setter, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo fallbackMethod = clazz.GetMethod(attr.Fallback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (setterMethod == null || fallbackMethod == null) {
                GD.PrintErr($"WARN: Serialiser.RegisterSerialiseData() : Missing setter or fallback method for {clazz.Name}.{field.Name}");
                continue;
            }
            
            serialiseData[attr.Key] = SmartSerialData.From(
                () => throw new InvalidOperationException("Use the overload that accepts the instance for the GETTER."),
                value => throw new InvalidOperationException("Use the overload that accepts the instance for the SETTER."),
                () => throw new InvalidOperationException("Use the overload that accepts the instance for the FALLBACK.")
            ).WithInstanceGetter((instance) => field.GetValue(instance)).WithInstanceSetter(
                (instance, value) => {
                    object convertedValue = Convert.ChangeType(value, setterMethod.GetParameters()[0].ParameterType);
                    setterMethod.Invoke(instance, new[] { convertedValue });
                }
            ).WithInstanceFallback(
                instance => {
                    fallbackMethod.Invoke(instance, null);
                }
            );
        }

        foreach (KeyValuePair<string, SmartSerialData> data in serialiseData) GD.Print($"For {clazz.Name} : {data.Key} : {data.Value}");

        _serialiseDataCache.Add(clazz, serialiseData);
    }


    public static Type GetObjectClass(string tag) => _registry.GetOrDefault(tag, null);

    private static string GetTag(Node3D node) {
        if (node.HasMeta(OBJECT_TAG)) return node.GetMeta(OBJECT_TAG).AsString();

        GD.PrintErr($"ERROR: ObjectAtlas.GetTag() : Node3D '{node.Name}' does not have the required meta tag '{OBJECT_TAG}'.");
        return "";
    }

    public static IObjectBase CreateObject(Node3D node) {
        string tag = GetTag(node);
        if (tag == "") return null;

        Type clazz = GetObjectClass(tag);
        if (clazz == null) throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Object class not found for tag '{tag}'.");
        return CreateObject(clazz, node);
    }

    public static IObjectBase CreateObject(Type type, Node3D node) {
        object instance;

        // Create a class of the given type with the given node. If the node is null, it's a data-only object and will not fully construct.
        if (node == null) {
            instance = _dataOnlyCache.GetOrCompute(
                type, () => {
                    object o = Activator.CreateInstance(type, null, true);
                    if (o is not IObjectBase obj)
                        throw new Exception($"ERROR: ObjectAtlas.CreateObject().GetOrCompute() : Created object is not of expected type 'IObjectBase'. Got '{o?.GetType()}'.");
                    return obj;
                }
            );
        } else instance = Activator.CreateInstance(type, node, false);

        if (instance is not IObjectBase obj)
            throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Created object is not of expected type 'IObjectBase'. Got '{instance?.GetType()}'.");
        return obj;
    }

    public static CreatedObject CreatedObjectFromData(string metaTag, string typeID, Dictionary<string, object> serialiseData) {
        CreatedObject createdObject = new();
        try {
            RigidBody3D node = Items.GetViaID(typeID).CreateInstance();
            IObjectBase objectBase = CreateObject(GetObjectClass(metaTag), node);
            createdObject.Object = objectBase;
            createdObject.Node = node;
            createdObject.Success = objectBase.BuildFromData(serialiseData);
        } catch (Exception e) {
            createdObject.Success = false;
            createdObject.Node?.QueueFree();
            GD.PrintErr($"ERROR: ObjectAtlas.CreatedObjectFromData() : Failed to create object from data. Exception: {e.Message}");
        }

        return createdObject;
    }

    public static CreatedObject CreatedObjectFromJson(string json) {
        Serialiser.ObjectSaveData data = DeserialiseObject(json);
        return CreatedObjectFromData(data.MetaTag, data.TypeID, data.Data);
    }

    public static Serialiser.ObjectSaveData DeserialiseObject(string json) {
        Serialiser.ObjectSaveData obj = Serialiser.Deserialise<Serialiser.ObjectSaveData>(json);
        obj.SanitiseToObjects();
        return obj;
    }

    public static IObjectBase DeserialiseDataWithoutNode(string json) {
        Serialiser.ObjectSaveData obj = DeserialiseObject(json);
        IObjectBase objBase = CreateObject(GetObjectClass(obj.MetaTag), null);
        return objBase;
    }

    public class CreatedObject {
        public bool Success { get; set; }
        public IObjectBase Object { get; set; }
        public Node Node { get; set; }

        public override string ToString() {
            return $"CreatedObject[Success={Success}, Object={Object}, Node={Node}]";
        }
    }
}