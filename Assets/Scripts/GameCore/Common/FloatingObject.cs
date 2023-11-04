using Common;
using UnityEngine;

namespace GameCore.Common
{
    public class FloatingObject : MonoBehaviour
    {
        [SerializeField] private float _floatingHeight;
        [SerializeField] private float _floatingSpeed;

        private float _startY;
        
        private void Awake()
        {
            _startY = transform.localPosition.y;
        }

        private void Update()
        {
            float t = Mathf.Sin(Time.time * _floatingSpeed);
            transform.localPosition = transform.localPosition.WithY(_startY + t * _floatingHeight);
        }
    }
}