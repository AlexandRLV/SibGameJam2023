using System.Diagnostics.CodeAnalysis;
using Common.DI;
using GameCore.Character.Visuals;
using GameCore.Enemies.EnemyObject;
using GameCore.Enemies.NewEnemy.Parameters;
using GameCore.Player;
using LocalMessages;
using UnityEngine;

namespace GameCore.Enemies.NewEnemy
{
    public class NewEnemyVision : MonoBehaviour
    {
        [field: SerializeField] public float HeadRotationAngle { get; set; }
        public bool IsAlarmed { get; private set; }

        [SerializeField] private bool _debug;
        [SerializeField] private LayerMask _raycastMask;
        [SerializeField] private MarkController _markController;
        [SerializeField] private EnemyFOV _enemyFOV;
        [SerializeField] private EnemyTimersParameters _timersParameters;
        
        [Inject] private LocalMessageBroker _messageBroker;

        private bool _initialized;
        private IPlayer _player;
        private EnemyViewPreset _viewPreset;
        
        private bool _canSeeTarget;
        private bool _isProvoked;
        private float _timer;

        private bool _hasHead;
        private Transform _head;
        
        private Vector3 _lookCenter;
        private Vector3 _lookBehindPoint;
        private Vector3 _lookDirection;
        private Transform _playerTransform;
        
        public void Initialize(IPlayer player, EnemyViewPreset viewPreset, CharacterVisuals visuals)
        {
            GameContainer.InjectToInstance(this);
            
            _enemyFOV.Init(viewPreset.viewAngle);
            _enemyFOV.SetColor(viewPreset.normalConeColor);
            
            _player = player;
            _viewPreset = viewPreset;
            _initialized = true;

            _head = visuals.Head;
            _hasHead = _head != null;
            
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
        }

        private void Update()
        {
            if (!_initialized) return;
            
            UpdateIndication();
            if (IsAlarmed)
            {
                _enemyFOV.Disable();
                return;
            }

            UpdateVision();
            _enemyFOV.DrawFOV(_viewPreset.viewDistance, _viewPreset.viewAngle, _raycastMask);
            _enemyFOV.transform.localRotation = Quaternion.Euler(0f, HeadRotationAngle, 0f);
            
            bool canSeeTarget = CheckCanSeeTarget();
            UpdateAlarmTimer(canSeeTarget);
        }

        private void LateUpdate()
        {
            if (!_hasHead) return;

            var additionalRotation = Quaternion.Euler(0f, HeadRotationAngle, 0f);
            _head.localRotation *= additionalRotation;
        }

        private void UpdateIndication()
        {
            _markController.SetProvokedMarkState(_isProvoked && !IsAlarmed);
            _markController.SetAlarmMarkState(IsAlarmed);
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void UpdateVision()
        {
            var rotation = Quaternion.Euler(0f, HeadRotationAngle, 0f);
            var forward = rotation * transform.forward;
            var right = rotation * transform.right;
            
            _lookCenter = transform.position + Vector3.up * _viewPreset.viewStartOffset;
            float distanceBack = _viewPreset.viewStartRadius / Mathf.Tan(_viewPreset.viewAngle * Mathf.Rad2Deg);
            _lookBehindPoint = _lookCenter - forward * distanceBack;
            _playerTransform = _player.CurrentMovement == null ? null : _player.CurrentMovement.transform;
            
#if UNITY_EDITOR
            if (!_debug) return;
            
            var forwardPoint = _lookCenter + forward * _viewPreset.viewDistance;
            Debug.DrawLine(_lookCenter, forwardPoint);
            float sideOffset = Mathf.Tan(_viewPreset.viewAngle * Mathf.Rad2Deg) * _viewPreset.viewDistance +
                               _viewPreset.viewStartRadius;
                
            Debug.DrawLine(_lookCenter + right, forwardPoint + right * sideOffset);
            Debug.DrawLine(_lookCenter + Vector3.up, forwardPoint + Vector3.up * sideOffset);
            Debug.DrawLine(_lookCenter - right, forwardPoint - right * sideOffset);
            Debug.DrawLine(_lookCenter - Vector3.up, forwardPoint - Vector3.up * sideOffset);
#endif
        }

        private bool CheckCanSeeTarget()
        {
            if (_playerTransform == null) return false;
            
            var playerPosition = _playerTransform.position;
            float distance = Vector3.Distance(playerPosition, _lookCenter);
            if (distance < _viewPreset.autoDetectRadius)
                return true;

            if (distance > _viewPreset.viewDistance)
                return false;

            var lookBehindDirection = playerPosition - _lookBehindPoint;
            float angle = Vector3.Angle(_lookDirection, lookBehindDirection);
            if (angle > _viewPreset.viewAngle) return false;

            var raycastDirection = playerPosition + Vector3.up - _lookCenter;
            if (!Physics.Raycast(_lookCenter, raycastDirection, out var hit, distance, _raycastMask))
                return true;

            return hit.transform.root == _playerTransform.root;
        }

        private void UpdateAlarmTimer(bool canSeeTarget)
        {
            if (!canSeeTarget && _canSeeTarget)
            {
                _timer = _timersParameters.disableProvokedSeconds;
                _canSeeTarget = false;
            }

            if (canSeeTarget && !_canSeeTarget)
            {
                _timer = _timersParameters.triggerAlarmSeconds;
                _canSeeTarget = true;
                _isProvoked = true;
            }
            
            if (!_canSeeTarget)
            {
                if (!_isProvoked) return;

                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _isProvoked = false;
                    return;
                }
            }

            _isProvoked = true;
            
            _timer -= Time.deltaTime;
            if (_timer > 0) return;
            
            IsAlarmed = true;
            _enemyFOV.SetColor(_viewPreset.alertConeColor);
            
            var message = new PlayerDetectedMessage();
            _messageBroker.Trigger(ref message);
        }

        private void OnPlayerDetected(ref PlayerDetectedMessage message)
        {
            IsAlarmed = true;
            _enemyFOV.SetColor(_viewPreset.alertConeColor);
        }
    }
}