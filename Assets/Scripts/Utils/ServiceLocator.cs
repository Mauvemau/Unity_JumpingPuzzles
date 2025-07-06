using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator {
    private static readonly Dictionary<Type, object> Services = new();

    public static void SetService<T>(T service, bool overrideIfFound = false) {
        Debug.Log($"{typeof(ServiceLocator)}: Service registered - {typeof(T)}");
        if (!Services.TryAdd(typeof(T), service) && overrideIfFound) {
            Services[typeof(T)] = service;
        }
    }

    public static bool TryGetService<T>(out T service) where T : class {
        if (Services.TryGetValue(typeof(T), out var serviceObject) && serviceObject is T tService) {
            service = tService;
            return true;
        }

        service = null;
        return false;
    }
    
    public static bool RemoveService<T>() {
        return Services.Remove(typeof(T));
    }

    
    public static void Clear() {
        Services.Clear();
    }
}
