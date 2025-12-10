using System;
using System.Collections.Generic;
using R3;
using UnityEngine.Assertions;

namespace HK
{
    public static class TinyServiceLocator
    {
        private static readonly Dictionary<Type, (object service, int version)> services = new();

        private static readonly Dictionary<Type, Dictionary<string, (object service, int version)>> namedServices = new();

        private static int currentVersion = 0;

        public static IDisposable Register<T>(T service)
        {
            Assert.IsFalse(services.ContainsKey(typeof(T)), $"Service already registered: {typeof(T)}");
            var v = currentVersion++;
            services[typeof(T)] = (service, v);
            return Disposable.Create((services, v), static (t) =>
            {
                var (services, v) = t;
                if (services[typeof(T)].version != v)
                {
                    return;
                }
                services.Remove(typeof(T));
            });
        }

        public static IDisposable Register<T>(string name, T service)
        {
            if (!namedServices.ContainsKey(typeof(T)))
            {
                namedServices[typeof(T)] = new Dictionary<string, (object service, int version)>();
            }
            Assert.IsFalse(namedServices[typeof(T)].ContainsKey(name), $"Service already registered: {typeof(T)} with name: {name}");
            var v = currentVersion++;
            namedServices[typeof(T)][name] = (service, v);
            return Disposable.Create((namedServices[typeof(T)], name, v), static (t) =>
            {
                var (namedServicesOfT, name, v) = t;
                if (namedServicesOfT[name].version != v)
                {
                    return;
                }
                namedServicesOfT.Remove(name);
            });
        }

        public static T Resolve<T>()
        {
            Assert.IsTrue(services.ContainsKey(typeof(T)), $"Service not found: {typeof(T)}");
            return (T)services[typeof(T)].service;
        }

        public static T Resolve<T>(string name)
        {
            Assert.IsTrue(namedServices.ContainsKey(typeof(T)), $"Service not found: {typeof(T)}");
            Assert.IsTrue(namedServices[typeof(T)].ContainsKey(name), $"Service not found: {typeof(T)}");
            return (T)namedServices[typeof(T)][name].service;
        }

        public static bool TryResolve<T>(out T service)
        {
            if (services.ContainsKey(typeof(T)))
            {
                service = (T)services[typeof(T)].service;
                return true;
            }
            service = default;
            return false;
        }

        public static bool TryResolve<T>(string name, out T service)
        {
            if (namedServices.ContainsKey(typeof(T)) && namedServices[typeof(T)].ContainsKey(name))
            {
                service = (T)namedServices[typeof(T)][name].service;
                return true;
            }
            service = default;
            return false;
        }

        public static bool Contains<T>()
        {
            return services.ContainsKey(typeof(T));
        }

        public static bool Contains<T>(string name)
        {
            return namedServices.ContainsKey(typeof(T)) && namedServices[typeof(T)].ContainsKey(name);
        }
    }
}