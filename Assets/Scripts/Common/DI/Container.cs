using System;
using System.Collections.Generic;

namespace Common.DI
{
    public sealed class Container
    {
        private Dictionary<Type, object> _registrations = new();

        public void Register<T>(T value)
        {
            var type = typeof(T);
            if (_registrations.ContainsKey(type))
                throw new ArgumentException("Trying to register already registered type!");
            
            _registrations.Add(type, value);
        }

        public T Resolve<T>() => _registrations.TryGetValue(typeof(T), out object value) ? (T)value : default;

        public bool HasRegistration<T>() => _registrations.ContainsKey(typeof(T));

        // Methods for automatic injection
        public object Resolve(Type type) => _registrations.GetValueOrDefault(type);
        public bool HasRegistration(Type type) => _registrations.ContainsKey(type);
    }
}