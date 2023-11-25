using UnityEngine;

namespace GameCore.Camera
{
    public class GameCamera : MonoBehaviour
    {
        public CameraFollowTarget FollowTarget => _followTarget;
        public Transform Camera => _camera;
        
        [SerializeField] private CameraFollowTarget _followTarget;
        [SerializeField] private Transform _camera;

        public void SetTarget(Transform target)
        {
            _followTarget.SetTarget(target);
        }

        private void OnDestroy()
        {
            Debug.Log("Game camera destroy");
        }

        public void ClearTarget()
        {
            _followTarget.ClearTarget();
            _followTarget.Height = 0f;
        }
    }
}