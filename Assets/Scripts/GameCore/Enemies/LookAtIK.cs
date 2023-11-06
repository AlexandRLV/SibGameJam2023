using UnityEngine;

namespace GameCore.Enemies
{
    public class LookAtIK : MonoBehaviour
    {
        [SerializeField] private Transform _head;

        [SerializeField] private float _lookAtDistance;
        [SerializeField] private float _lookAtRange;
        [SerializeField] private float _lookAtSpeed;

        private bool _hasTarget;

        private void LateUpdate()
        {
            if (_hasTarget)
                return;
            
            float t = Mathf.Sin(Time.time * _lookAtSpeed);

            var position = transform.position + transform.forward * _lookAtDistance;
            var right = transform.right * (t * _lookAtRange);
            position += right;
            position.y = _head.position.y;
            _head.LookAt(position);
        }

        public void SetTarget(Vector3 target)
        {
            _hasTarget = true;
            _head.LookAt(target);
        }
    }
}