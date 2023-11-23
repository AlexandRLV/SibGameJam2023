﻿using System.Collections;
using Common;
using Common.DI;
using GameCore.Input;
using UnityEngine;

namespace GameCore.Camera
{
    public class CameraFollowTarget : MonoBehaviour
    {
        public float Height { get; set; }

        [SerializeField] private CameraSettings _cameraSettings;

        [SerializeField] private float _minAngle;
        [SerializeField] private float _maxAngle;

        private bool _inPause;
        
        private bool _hasTarget;
        private Transform _target;

        private InputState _inputState;

#region Private Methods
        private IEnumerator Start()
        {
            while (GameContainer.InGame == null || !GameContainer.InGame.CanResolve<InputState>())
            {
                yield return null;
            }

            _inputState = GameContainer.InGame.Resolve<InputState>();
        }

        private void Update()
        {
            if (_inPause) return;
            
            CheckLockCamera();
            
            if (_inputState == null) return;
            if (!_hasTarget) return;

            transform.position = _target.position + Vector3.up * Height;

            // X and Y swapped because mouse movement along X axis means camera rotation around Y axis and so on
            float angleX = -_inputState.camera.y * _cameraSettings.sensitivity;
            float angleY = _inputState.camera.x * _cameraSettings.sensitivity;

            if (_cameraSettings.invertX) angleX *= -1;
            if (_cameraSettings.invertY) angleY *= -1;

            var euler = transform.eulerAngles;
            euler.x += angleX;
            euler.y += angleY;
            RestrictMinMaxAngles(ref euler);

            euler.z = 0f;
            transform.eulerAngles = euler;
        }

        private void RestrictMinMaxAngles(ref Vector3 euler)
        {
            if (euler.x > 180f && euler.x < 360f - _minAngle) euler.x = 360f - _minAngle;
            if (euler.x < 180f && euler.x > _maxAngle) euler.x = _maxAngle;
        }

        private void CheckLockCamera()
        {
            if (Cursor.visible)
            {
                if (Application.isFocused && (UnityEngine.Input.GetMouseButtonDown(0) || UnityEngine.Input.GetMouseButtonDown(1) ||
                     UnityEngine.Input.GetKeyDown(KeyCode.M)))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else
            {
                if (!Application.isFocused || UnityEngine.Input.GetKeyDown(KeyCode.M) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
#endregion

#region Public Methods
        public void SetTarget(Transform target)
        {
            _hasTarget = true;
            _target = target;
        }

        public void ClearTarget()
        {
            _hasTarget = false;
            _target = null;
        }

        public void SetInPause(bool value)
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            _inPause = value;
        }
#endregion
    }
}