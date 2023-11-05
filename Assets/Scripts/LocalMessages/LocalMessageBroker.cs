using System;
using System.Collections.Generic;

namespace LocalMessages
{
    public delegate void ActionRef<T>(ref T value);
    
    public class LocalMessageBroker
    {
        private interface IListenersContainer { }
        
        private class ListenersContainer<T> : IListenersContainer where T : struct
        {
            public readonly List<ActionRef<T>> listeners = new();
        }

        private Dictionary<Type, IListenersContainer> _handlers = new();

        public void Subscribe<T>(ActionRef<T> callback) where T : struct
        {
            var type = typeof(T);
            ListenersContainer<T> container;
            if (_handlers.TryGetValue(type, out var iContainer))
            {
                container = iContainer as ListenersContainer<T>;
                if (container == null)
                    throw new ArgumentException($"Subscribe to local event error: wrong receiver type: {type.FullName}");
            }
            else
            {
                container = new ListenersContainer<T>();
                _handlers.Add(type, container);
            }
            
            container.listeners.Add(callback);
        }

        public void Unsubscribe<T>(ActionRef<T> callback) where T : struct
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var iContainer))
                return;
            
            if (iContainer is not ListenersContainer<T> container)
                throw new ArgumentException($"Unsubscribe from local event error: wrong receiver type: {type.FullName}");

            container.listeners.Remove(callback);
        }

        public void Trigger<T>(ref T message) where T : struct
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var iContainer))
                return;
            
            if (iContainer is not ListenersContainer<T> container)
                throw new ArgumentException($"Trigger local event error: wrong receiver type: {type.FullName}");
            
            foreach (var listener in container.listeners)
            {
                listener.Invoke(ref message);
            }
        }
    }
}