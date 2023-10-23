using System;
using System.Collections.Generic;

namespace SibGameJam.Common.LocalMessages
{
    public static class LocalMessageBroker
    {
        public delegate void ActionRef<T>(ref T message) where T : struct, ILocalMessage;
        
        private interface IListenerContainer { }
        
        private class ListenerContainer<T> : IListenerContainer where T : struct, ILocalMessage
        {
            public List<ActionRef<T>> Callbacks = new();
        }

        private static Dictionary<Type, IListenerContainer> _listeners = new();

        public static void Subscribe<T>(ActionRef<T> callback) where T : struct, ILocalMessage
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var container))
            {
                container = new ListenerContainer<T>();
                _listeners.Add(type, container);
            }

            if (container is not ListenerContainer<T> typedContainer)
                throw new ArgumentException($"Message broker contains wrong listener type for type {type.Name}!");

            typedContainer.Callbacks.Add(callback);
        }

        public static void Unsubscribe<T>(ActionRef<T> callback) where T : struct, ILocalMessage
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var container))
                return;

            if (container is not ListenerContainer<T> typedContainer)
                throw new ArgumentException($"Message broker contains wrong listener type for type {type.Name}!");

            typedContainer.Callbacks.Remove(callback);
        }

        public static void Trigger<T>(ref T message) where T : struct, ILocalMessage
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var container))
                return;
            
            if (container is not ListenerContainer<T> typedContainer)
                throw new ArgumentException($"Message broker contains wrong listener type for type {type.Name}!");

            foreach (var callback in typedContainer.Callbacks)
            {
                callback?.Invoke(ref message);
            }
        }
    }
}