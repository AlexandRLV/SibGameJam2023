using Cinemachine;
using UnityEngine;

namespace GameCore.Camera
{
    public class GameCamera : MonoBehaviour
    {
        public CameraFollowTarget FollowTarget => _followTarget;
        
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private CameraFollowTarget _followTarget;

        public void SetTarget(Transform target)
        {
            _followTarget.SetTarget(target);
        }

        public void ClearTarget()
        {
            _followTarget.ClearTarget();
            _followTarget.Height = 0f;
        }
    }
}