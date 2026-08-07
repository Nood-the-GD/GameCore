using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : IDisposable
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static ServiceRegistration Register(object instance)
    {
        if (instance == null)
        {
            Debug.LogError("[ServiceManager] Register called with a null instance.");
            return new ServiceRegistration(instance);
        }

        Add(instance.GetType(), instance);
        return new ServiceRegistration(instance);
    }

    public static ServiceUnregistration Unregister(object instance)
    {
        return new ServiceUnregistration(instance);
    }

    public static T Get<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;

        Debug.LogError($"[ServiceManager] No instance registered for {typeof(T).Name}.");
        return null;
    }

    public static bool TryGet<T>(out T service) where T : class
    {
        if (_services.TryGetValue(typeof(T), out var raw))
        {
            service = (T)raw;
            return true;
        }

        service = null;
        return false;
    }

    public static void Init()
    {
        _services.Clear();
    }

    internal static void Add(Type type, object instance)
    {
        if (instance == null)
        {
            Debug.LogError($"[ServiceManager] Cannot register {type.Name}: instance is null or does not implement {type.Name}.");
            return;
        }

        if (_services.ContainsKey(type))
            Debug.LogWarning($"[ServiceManager] {type.Name} is already registered. Overwriting previous instance.");

        _services[type] = instance;
    }

    internal static void Remove(Type type, object instance)
    {
        if (_services.TryGetValue(type, out var existing) && ReferenceEquals(existing, instance))
            _services.Remove(type);
    }

    internal static void RemoveAll(object instance)
    {
        var keysToRemove = new List<Type>();
        foreach (var pair in _services)
        {
            if (ReferenceEquals(pair.Value, instance))
                keysToRemove.Add(pair.Key);
        }

        foreach (var key in keysToRemove)
            _services.Remove(key);
    }

    public void Dispose()
    {
        _services.Clear();
    }
}

public readonly struct ServiceRegistration
{
    private readonly object _instance;

    internal ServiceRegistration(object instance)
    {
        _instance = instance;
    }

    public ServiceRegistration As<T>() where T : class
    {
        if (_instance == null) return this;

        if (_instance is not T typed)
        {
            Debug.LogError($"[ServiceManager] {_instance.GetType().Name} does not implement {typeof(T).Name}.");
            return this;
        }

        ServiceManager.Add(typeof(T), typed);
        return this;
    }

    public ServiceRegistration AsImplementedInterfaces()
    {
        if (_instance == null) return this;

        foreach (var interfaceType in _instance.GetType().GetInterfaces())
            ServiceManager.Add(interfaceType, _instance);

        return this;
    }
}

public readonly struct ServiceUnregistration
{
    private readonly object _instance;

    internal ServiceUnregistration(object instance)
    {
        _instance = instance;
    }

    public ServiceUnregistration As<T>() where T : class
    {
        if (_instance != null)
            ServiceManager.Remove(typeof(T), _instance);

        return this;
    }

    public void AsAll()
    {
        if (_instance != null)
            ServiceManager.RemoveAll(_instance);
    }
}
