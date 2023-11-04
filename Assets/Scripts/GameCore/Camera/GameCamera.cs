using Cinemachine;
using UnityEngine;

namespace GameCore.Camera
{
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        public void SetTarget(Transform target)
        {
            _virtualCamera.Follow = target;
            _virtualCamera.LookAt = target;
        }

        public void ClearTarget()
        {
            _virtualCamera.LookAt = null;
            _virtualCamera.Follow = null;
        }
    }
}