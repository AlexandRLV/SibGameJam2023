using System.Collections.Generic;

namespace Common
{
    public static class ArrayPool<T>
    {
        private static readonly Dictionary<int, Stack<T[]>> _pool = new();
        
        public static T[] New(int length)
        {
            if (!_pool.ContainsKey(length))
                _pool.Add(length, new Stack<T[]>());

            if (_pool[length].Count == 0)
                _pool[length].Push(new T[length]);

            var array = _pool[length].Pop();
            return array;
        }

        public static void Free(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default;
            }

            _pool[array.Length].Push(array);
        }
    }
}