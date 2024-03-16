using System.Collections.Generic;
using Common.DI;
using GameCore.Enemies.RouteControl;
using GameCore.LevelObjects;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
using Networking.Client;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Enemies.EnemyObject
{
    public class EnemyController : MonoBehaviour, ICheckPositionObject
    {
        public Vector3 CheckPosition { get; private set; }

        [Header("Main")]
        [SerializeField] private EnemyMovementType movementType = EnemyMovementType.WaypointsSequential;
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
        private bool _isPlayerDetected;
        private bool _isAlert;
        private bool _canTriggerAlert;

        private bool _hasSeenCharacter;

        private float _timer;
        private bool _canTurn;

        private EnemyTargetScaner _enemyScan;
        private EnemyMovement _enemyMovement;
        private EnemyFOV _enemyFOV;
        private Transform _currentTarget;
        private LocalMessageBroker _messageBroker;
        private MarkController _markController;

        private void Start()
        {
            if (movementType == EnemyMovementType.NoWalk)
                Init(null);

            CheckPosition = transform.position;
            
            GameContainer.InjectToInstance(this);
            _levelObjectService.RegisterEnemy(this);
        }

        public void Init(List<Waypoint> movePoints)
        {
            _enemyScan = GetComponentInChildren<EnemyTargetScaner>();
            _markController = GetComponentInChildren<MarkController>();
            
            _enemyMovement = GetComponent<EnemyMovement>();
            _enemyMovement.movePoints = movePoints;
            _enemyMovement.Init(moveSpeed);
            
            _enemyFOV = GetComponentInChildren<EnemyFOV>();
            _enemyFOV.Init(_enemyScan.ViewAngle);
            _enemyFOV.SetColor(normalConeColor);

            _currentTarget = null;
            
            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
        }

        private void FixedUpdate()
        {
            if (_isAlert) return;
            
            _currentTarget = _enemyScan.GetNearestTarget();

            if (EnemyMovementType.WaypointsSequential == movementType)
                _enemyMovement.SequentialWaypointsMovement();
            else if (EnemyMovementType.WaypointsClockwise == movementType)
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

        public void UnDetectPlayer()
        {
            _markController.ResetMarks();
            _isPlayerDetected = false;
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