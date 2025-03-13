using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Godot;

public static class ObjectAtlas {
    private static readonly SmartDictionary<string, Type> _registry = new();

    private const string
        OBJECT_TAG = "object_tag";
    
    static ObjectAtlas() => RegisterAll();
    
    private static void RegisterAll() {
        // Automatically registers all classes that inherit from ObjectBase<>
        IEnumerable<Type> objectTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IObjectBase).IsAssignableFrom(t));

        foreach (Type type in objectTypes)
            Register(type);
    }

    public static void Register([NotNull] Type clazz) {
        ArgumentNullException.ThrowIfNull(clazz);
        if (!typeof(IObjectBase).IsAssignableFrom(clazz)) 
            throw new Exception($"ERROR: ObjectAtlas.Register() : Class '{clazz.Name}' must inherit from ObjectBase<?>.");
        
        GD.Print($"INFO: ObjectAtlas.Register() : Registering object class: {clazz.Name}...");

        try {
            object dummy = Activator.CreateInstance(clazz, new object[] {null});
            if (dummy is not IObjectBase obj)
                throw new Exception($"ERROR: ObjectAtlas.Register() : Created object is not of expected type 'IObjectBase'. Got '{dummy?.GetType()}'.");
            
            _registry.Add(obj.GetObjectTag(), clazz);
        }
        catch (Exception e) {
            GD.PrintErr($"ERROR: ObjectAtlas.Register() : Failed to create instance of class '{clazz.Name}'. Exception: {e.Message}");
        }
    }
    public static Type GetObjectClass(string tag) => _registry.GetOrDefault(tag, null);

    private static string GetTag(Node3D node) {
        if (node.HasMeta(OBJECT_TAG))
            return node.GetMeta(OBJECT_TAG).AsString();
        
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
        object instance = Activator.CreateInstance(type, node);
        if (instance is not IObjectBase obj) throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Created object is not of expected type 'IObjectBase'. Got '{instance?.GetType()}'.");
        return obj;
    }
    
    public static T CreateObject<T, U>(U node) where T : ObjectBase<U> where U : Node3D {
        string tag = GetTag(node);
        Type clazz = GetObjectClass(tag);
        if (clazz == null) throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Object class not found for tag '{tag}'.");
        object instance = Activator.CreateInstance(clazz, node);
        if (instance is not T obj) throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Created object is not of expected type '{typeof(T)}'. Got '{instance?.GetType()}'.");
        return obj;
    }

    public static CreatedObject CreatedObjectFromData(string metaTag, string scenePath, SmartDictionary<string, (Variant, Action<Variant>)> serialiseData) {
        CreatedObject createdObject = new();
        try {
            Node3D node = Loader.SafeInstantiate<Node3D>(scenePath, true);
            IObjectBase objectBase = CreateObject(GetObjectClass(metaTag), node);
            createdObject.Object = objectBase;
            createdObject.Node = node;
            createdObject.Success = objectBase.BuildFromData(serialiseData);
        }
        catch (Exception e) {
            createdObject.Success = false;
            GD.PrintErr($"ERROR: ObjectAtlas.CreatedObjectFromData() : Failed to create object from data. Exception: {e.Message}");
        }
        return createdObject;
    }
    
    
    public class CreatedObject {
        public bool Success { get; set; }
        public IObjectBase Object { get; set; }
        public Node Node { get; set; }
    }
}