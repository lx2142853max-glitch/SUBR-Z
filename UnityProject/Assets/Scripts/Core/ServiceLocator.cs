using System;
using System.Collections.Generic;
using UnityEngine;

namespace SUBR.Core
{
    /// <summary>Minimal DI — Register in bootstrap, Resolve anywhere.</summary>
    public static class ServiceLocator
    {
        static readonly Dictionary<Type, object> _map = new Dictionary<Type, object>();

        public static void Register<T>(T instance) where T : class
        {
            _map[typeof(T)] = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public static T Resolve<T>() where T : class
        {
            if (_map.TryGetValue(typeof(T), out var o) && o is T t)
                return t;
            throw new InvalidOperationException($"Service not registered: {typeof(T).Name}");
        }

        public static bool TryResolve<T>(out T service) where T : class
        {
            if (_map.TryGetValue(typeof(T), out var o) && o is T t)
            {
                service = t;
                return true;
            }
            service = null;
            return false;
        }

        public static void Clear() => _map.Clear();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void AutoClear() => Clear();
    }
}
