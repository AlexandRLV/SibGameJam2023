using Common.DI;
using GameCore.Camera;
using UnityEngine;

namespace GameCore.Common
{
    public class RotateToCamera : MonoBehaviour
    {
        private bool _hasCamera;
        private Transform _cameraTransform;

        private void Update()
        {
            if (!_hasCamera)
            {
                if (!GameContainer.InGame.CanResolve<GameCamera>())
                    return;

                var gameCamera = GameContainer.InGame.Resolve<GameCamera>();
                _cameraTransform = gameCamera.Camera;
                _hasCamera = true;
            }
            
            var direction = _cameraTransform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}