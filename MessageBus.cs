using System;
using System.Collections.Generic;
using UnityEngine;

public static class MessageBus
{
    private static readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(Action<T> handler) where T : struct
    {
        if (handler == null)
        {
            Debug.LogError($"[MessageBus] Subscribe<{typeof(T).Name}> called with null handler.");
            return;
        }

        var eventType = typeof(T);
        _handlers.TryGetValue(eventType, out var existing);
        _handlers[eventType] = Delegate.Combine(existing, handler);
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        if (handler == null) return;

        var eventType = typeof(T);
        if (!_handlers.TryGetValue(eventType, out var existing)) return;

        var updated = Delegate.Remove(existing, handler);
        if (updated == null)
            _handlers.Remove(eventType);
        else
            _handlers[eventType] = updated;
    }

    public static void Publish<T>(T message) where T : struct
    {
        var eventType = typeof(T);
        if (!_handlers.TryGetValue(eventType, out var del) || del == null) return;

        foreach (var subscriber in del.GetInvocationList())
        {
            try
            {
                ((Action<T>)subscriber).Invoke(message);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MessageBus] Subscriber to {eventType.Name} threw an exception: {ex}");
            }
        }
    }

    public static void Init()
    {
        _handlers.Clear();
    }
}
