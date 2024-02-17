using System;
using System.Collections.Generic;

namespace Common.DI
{
    public sealed class Container
    {
        private readonly Dictionary<Type, object> _registrations = new();

        public void Register<T>(T value)
        {
            var type = typeof(T);
            if (!_registrations.TryAdd(type, value))
                throw new ArgumentException("Trying to register already registered type!");
        }

        public T Resolve<T>() => _registrations.TryGetValue(typeof(T), out object value) ? (T)value : default;

        // Methods for automatic injection
        public object Resolve(Type type) => _registrations.GetValueOrDefault(type);
        public bool HasRegistration(Type type) => _registrations.ContainsKey(type);
        
        public void Dispose()
        {
            foreach (var registration in _registrations)
            {
                if (registration.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            _registrations.Clear();
        }
    }
}