using UnityEngine;

namespace GameCore.ObjectsPool
{
    public struct ReturnOnTimeContainer<T> where T : MonoBehaviour
    {
        public T value;
        public float timer;
    }
}