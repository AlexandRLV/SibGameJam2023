using System.Collections.Generic;
using UnityEngine;

namespace GameCore.ObjectsPool
{
    public class ObjectsPoolBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T _prefab;

        private Transform _parent;

        private Queue<T> _queue;
        private List<ReturnOnTimeContainer<T>> _returnOnTimeContainers;
        
        private void Awake()
        {
            _queue = new Queue<T>();
            _returnOnTimeContainers = new List<ReturnOnTimeContainer<T>>();

            var parentGO = new GameObject("Spawned Objects");
            parentGO.transform.parent = transform;
            parentGO.transform.localPosition = Vector3.zero;

            _parent = parentGO.transform;
            
            _prefab.gameObject.SetActive(false);
        }

        private void Update()
        {
            for (int i = _returnOnTimeContainers.Count - 1; i >= 0; i--)
            {
                var container = _returnOnTimeContainers[i];
                container.timer -= Time.deltaTime;
                if (container.timer > 0f)
                {
                    _returnOnTimeContainers[i] = container;
                    continue;
                }
				
                Return(container.value);
                _returnOnTimeContainers.RemoveAt(i);
            }
        }
        
        public T Get()
        {
            return _queue.TryDequeue(out var prefab) ? prefab : Instantiate(_prefab, _parent.transform);
        }

        public T GetForTime(float time)
        {
            var value = Get();
            ReturnOnTime(value, time);
            return value;
        }

        public void Return(T value)
        {
            value.gameObject.SetActive(false);
            value.transform.parent = _parent.transform;
            _queue.Enqueue(value);
        }

        public void ReturnOnTime(T value, float time)
        {
            _returnOnTimeContainers.Add(new ReturnOnTimeContainer<T>
            {
                value = value,
                timer = time
            });
        }
    }
}