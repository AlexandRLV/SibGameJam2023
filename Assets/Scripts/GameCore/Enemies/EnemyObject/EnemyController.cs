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
        waypointsSequentalPatrolling,
        clockwise,
        noWalk
    }

    public class EnemyController : MonoBehaviour, ICheckPositionObject
    {
        public Vector3 CheckPosition { get; private set; }
        private SoundService soundService => GameContainer.Common.Resolve<SoundService>();

        [Header("Main")]
        [SerializeField] private MovementType movementType = MovementType.waypointsSequentalPatrolling;
        [SerializeField] private float timeToAlert;
        [SerializeField] private float questionTimeAfterDetect;
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private Color normalConeColor, alertConeColor;

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

        private GameClient _gameClient;

        private void Awake()
        {
            if (MovementType.noWalk == movementType)
                Init(null);

            CheckPosition = transform.position;
            _gameClient = GameContainer.Common.Resolve<GameClient>();
        
            GameContainer.InGame.Resolve<LevelObjectService>().RegisterEnemy(this);
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

            if (_currentTarget != null)
            {
                var player = GameContainer.InGame.Resolve<IPlayer>();
                DetectPlayer(player.MouseType, true);

                if (!_gameClient.IsConnected) return;
            
                Debug.Log("Send detect player dataframe");
                var dataframe = new EnemyDetectPlayerDataframe
                {
                    checkPosition = CheckPosition,
                    isDetect = true,
                };
                _gameClient.Send(ref dataframe);
            }
            else
            {
                if (_isPlayerDetected && _gameClient.IsConnected)
                {
                    Debug.Log("Send undetect player dataframe");
                    var dataframe = new EnemyDetectPlayerDataframe
                    {
                        checkPosition = CheckPosition,
                        isDetect = false,
                    };
                    _gameClient.Send(ref dataframe);
                }
            
                _isPlayerDetected = false;

                if (MovementType.waypointsSequentalPatrolling == movementType)
                    _enemyMovement.SequentalWaypointsMovement();
                else if (MovementType.clockwise == movementType)
                    _enemyMovement.ClockwiseWaypointsMovement();
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
            Debug.Log($"Enemy detected player, mouse type: {mouseType}, can alert: {canTriggerAlert}, already seen: {_hasSeenCharacter}");
            if (!_isPlayerDetected && !_hasSeenCharacter)
            {
                _hasSeenCharacter = true;
                soundService.PlaySound(mouseType == PlayerMouseType.ThinMouse ? SoundType.ThinDetect : SoundType.FatDetect);
            }
            _isPlayerDetected = true;
            _canTriggerAlert = canTriggerAlert;
        }

        public void UndetectPlayer()
        {
            Debug.Log("Enemy undetected player");
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
            Debug.Log("Player detected message, ALERT!!!");
            _isAlert = true;
            _enemyMovement.MoveToTarget(value.PlayerPosition);
            _enemyFOV.SetColor(alertConeColor);
            _markController.SetExclamationMark();

            var lookIk = GetComponentInChildren<LookAtIK>();
            if (lookIk != null)
            {
                lookIk.SetTarget(value.PlayerPosition);
            }
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
            {
                _markController.ResetMarks();
            }
            else
            {
                _remainingTimeToShowQuestion -= Time.deltaTime;
            }
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
        
            if (GameContainer.InGame.CanResolve<LevelObjectService>())
                GameContainer.InGame.Resolve<LevelObjectService>().UnregisterEnemy(this);
        }

        private void StartAlert()
        {
            Debug.Log("Starting alert by timer");
            if (!_canTriggerAlert)
            {
                Debug.Log("Cannot trigger alert, return");
                return;
            }

            if (_gameClient.IsConnected)
            {
                Debug.Log("Sending alert dataframe");
                var dataframe = new EnemyAlertPlayerDataframe
                {
                    playerPosition = _currentTarget.position
                };
                _gameClient.Send(ref dataframe);
            }
        
            soundService.PlaySound(SoundType.Alert);
            var message = new PlayerDetectedMessage();
            message.PlayerPosition = _currentTarget.position;
            _messageBroker.Trigger(ref message);
        }
    }
}