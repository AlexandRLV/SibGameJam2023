using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NetFrame.Utils
{
    public class ThreadSafeContainer<T> : IEnumerable<T> where T : class
    {
        public int Count => Queue.Count;

        private ConcurrentQueue<T> Queue { get; }
    
        public ThreadSafeContainer()
        {
            Queue = new ConcurrentQueue<T>();
        }
        
        public void Add(T item)
        {
            Queue.Enqueue(item);
        }
    
        public struct Enumerator : IEnumerator<T>
        {
            private readonly ConcurrentQueue<T> _queue;
            private T _current;

            public Enumerator(ConcurrentQueue<T> queue)
            {
                _queue = queue;
                _current = default;
            }

            public bool MoveNext()
            {
                return _queue.TryDequeue(out _current);
            }

            public void Reset()
            {
                _current = default;
            }
            
            public T Current => _current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
            
            }
        }
        
        public Enumerator GetEnumerator() => new Enumerator(Queue);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}