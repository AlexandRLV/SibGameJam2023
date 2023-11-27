using System.Collections.Generic;
using Common.DI;
using GameCore.Camera;
using GameCore.Enemies.RouteControl;
using GameCore.LevelObjects;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
using Networking;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Enemies.EnemyObject
{
    public enum MovementType
    {
        WaypointsSequentalPatrolling,
        Clockwise,
        NoWalk
    }

    public class EnemyController : MonoBehaviour, ICheckPositionObject
    {
        public Vector3 CheckPosition { get; private set; }

        [Header("Main")]
        [SerializeField] private MovementType movementType = MovementType.WaypointsSequentalPatrolling;
        [SerializeField] private float timeToAlert;
        [SerializeField] private float questionTimeAfterDetect;
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private Color normalConeColor, alertConeColor;

        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private SoundService _soundService;
        [Inject] private LevelObjectService _levelObjectService;

        private float _remainingTimeToAlert;
        private float _remainingTimeToShowQuestion;
        private bool _isPlayerDetected = false;
        private bool _isAlert = false;
        private bool _canTriggerAlert;

        private float _timer;
        private bool _canTurn;

        private EnemyTargetScaner _enemyScan;
        private EnemyMovement _enemyMovement;
        private EnemyFOV _enemyFOV;
        private Transform _currentTarget;
        private LocalMessageBroker _messageBroker;
        private MarkController _markController;

        private bool _hasSeenCharacter;

        private void Start()
        {
            if (movementType == MovementType.NoWalk)
                Init(null);

            CheckPosition = transform.position;
            
            GameContainer.InjectToInstance(this);
            _levelObjectService.RegisterEnemy(this);
        }

        public void Init(List<Waypoint> movePoints)
        {
            _enemyScan = GetComponentInChildren<EnemyTargetScaner>();
            _enemyMovement = GetComponent<EnemyMovement>();
            _enemyFOV = GetComponentInChildren<EnemyFOV>();
            _markController = GetComponentInChildren<MarkController>();
            _enemyMovement.movePoints = movePoints;
            _enemyMovement.Init(moveSpeed);
            _currentTarget = null;
            _enemyFOV.Init(_enemyScan.ViewAngle);
            _enemyFOV.SetColor(normalConeColor);

            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
        }

        private void FixedUpdate()
        {
            if (_isAlert) return;
            
            _currentTarget = _enemyScan.GetNearestTarget();

            if (MovementType.WaypointsSequentalPatrolling == movementType)
                _enemyMovement.SequentalWaypointsMovement();
            else if (MovementType.Clockwise == movementType)
                _enemyMovement.ClockwiseWaypointsMovement();
            
            if (_currentTarget != null)
            {
                var player = GameContainer.InGame.Resolve<IPlayer>();
                DetectPlayer(player.MouseType);

                if (!_gameClientData.IsConnected) return;
            
                var dataframe = new EnemyDetectPlayerDataframe
                {
                    checkPosition = CheckPosition,
                    isDetect = true,
                };
                _gameClient.Send(ref dataframe);
            }
            else
            {
                if (_isPlayerDetected && _gameClientData.IsConnected)
                {
                    var dataframe = new EnemyDetectPlayerDataframe
                    {
                        checkPosition = CheckPosition,
                        isDetect = false,
                    };
                    _gameClient.Send(ref dataframe);
                }
            
                _isPlayerDetected = false;
            }
        }

        private void Update()
        {
            _enemyFOV.DrawFOV(_enemyScan.ViewDistance, _enemyScan.ViewAngle, _enemyScan.ObstacleLayer);

            if (_isAlert) return;
            if (_isPlayerDetected && _isAlert == false)
            {
                _markController.SetQuestionMark();
                CountRemainingTimeToAlert();
                _remainingTimeToShowQuestion = questionTimeAfterDetect;
            }
            else
            {
                _remainingTimeToAlert = timeToAlert;
                CountRemainingTimeToShowQuestion();
            }
        }

        public void DetectPlayer(PlayerMouseType mouseType, bool canTriggerAlert = true)
        {
            if (!_isPlayerDetected && !_hasSeenCharacter)
            {
                _hasSeenCharacter = true;
                _soundService.PlaySound(mouseType == PlayerMouseType.ThinMouse ? SoundType.ThinDetect : SoundType.FatDetect);
            }
            _isPlayerDetected = true;
            _canTriggerAlert = canTriggerAlert;
        }

        public void UndetectPlayer()
        {
            _markController.ResetMarks();
            _isPlayerDetected = false;
        }

        private void LateUpdate()
        {
            if (!GameContainer.InGame.CanResolve<GameCamera>())
                return;
        
            var cameraService = GameContainer.InGame.Resolve<GameCamera>();
            if (cameraService != null && cameraService.Camera != null)
                _markController.LookAt(cameraService.Camera);
        }

        private void OnPlayerDetected(ref PlayerDetectedMessage value)
        {
            _isAlert = true;
            _enemyMovement.MoveToTarget(value.PlayerPosition);
            _enemyFOV.SetColor(alertConeColor);
            _markController.SetExclamationMark();

            var lookIk = GetComponentInChildren<LookAtIK>();
            if (lookIk != null)
                lookIk.SetTarget(value.PlayerPosition);
        }

        private void CountRemainingTimeToAlert()
        {
            _remainingTimeToAlert -= Time.deltaTime;
            if (_remainingTimeToAlert < 0)
                StartAlert();
        }

        private void CountRemainingTimeToShowQuestion()
        {
            if (_remainingTimeToShowQuestion < 0)
                _markController.ResetMarks();
            else
                _remainingTimeToShowQuestion -= Time.deltaTime;
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
        
            if (GameContainer.InGame.CanResolve<LevelObjectService>())
                GameContainer.InGame.Resolve<LevelObjectService>().UnregisterEnemy(this);
        }

        private void StartAlert()
        {
            if (!_canTriggerAlert)
                return;

            if (_gameClientData.IsConnected)
            {
                var dataframe = new EnemyAlertPlayerDataframe
                {
                    playerPosition = _currentTarget.position
                };
                _gameClient.Send(ref dataframe);
            }
        
            _soundService.PlaySound(SoundType.Alert);
            
            var message = new PlayerDetectedMessage
            {
                PlayerPosition = _currentTarget.position
            };
            _messageBroker.Trigger(ref message);
        }
    }
}