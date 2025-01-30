using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Godot;

public static class ObjectAtlas {
    private static readonly SmartDictionary<string, Type> _registry = new();

    static ObjectAtlas() {
        RegisterAll();
    }
    public static void Register([NotNull] Type clazz) {
        ArgumentNullException.ThrowIfNull(clazz);
        if (!typeof(IObjectBase).IsAssignableFrom(clazz)) 
            throw new Exception($"ERROR: ObjectAtlas.Register() : Class '{clazz.Name}' must inherit from ObjectBase<?>.");
        
        GD.Print($"Registering object class: {clazz.Name}");
        
        MethodInfo tagMethod = clazz.GetMethod("GetObjectTag", BindingFlags.Public | BindingFlags.Static);
        if (tagMethod == null) throw new Exception($"ERROR: ObjectAtlas.Register() : Object class must have a static method called GetObjectTag. Class: {clazz.Name}");
        _registry.Add((string) tagMethod.Invoke(null, null), clazz);
    }
    public static Type GetObjectClass(string tag) => _registry.GetOrDefault(tag, null);
    
    public static IObjectBase CreateObject(string tag, Node3D node) {
        Type clazz = GetObjectClass(tag);
        if (clazz == null) throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Object class not found for tag '{tag}'.");
        object instance = Activator.CreateInstance(clazz, node);
        if (instance is not IObjectBase obj) throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Created object is not of expected type 'IObjectBase'. Got '{instance?.GetType()}'.");
        return obj;
    }
    
    public static T CreateObject<T, U>(string tag, U node) where T : ObjectBase<U> where U : Node3D {
        Type clazz = GetObjectClass(tag);
        if (clazz == null) throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Object class not found for tag '{tag}'.");
        object instance = Activator.CreateInstance(clazz, node);
        if (instance is not T obj) throw new Exception($"ERROR: ObjectAtlas.CreateObject() : Created object is not of expected type '{typeof(T)}'. Got '{instance?.GetType()}'.");
        return obj;
    }
    
    private static void RegisterAll() {
        // Automatically registers all classes that inherit from ObjectBase<>
        IEnumerable<Type> objectTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IObjectBase).IsAssignableFrom(t));

        foreach (Type type in objectTypes) {
            Register(type);
        }
    }
}