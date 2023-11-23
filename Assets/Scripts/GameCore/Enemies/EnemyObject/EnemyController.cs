using Common;
using GameCore.Sounds;
using LocalMessages;
using System.Collections;
using System.Collections.Generic;
using GameCore.Camera;
using GameCore.Enemies;
using GameCore.LevelObjects;
using GameCore.Player;
using Networking;
using Networking.Dataframes.InGame;
using UnityEditor;
using UnityEngine;

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
            DetectPlayer(player.MouseType);

            if (!_gameClient.IsConnected) return;
            var dataframe = new EnemyDetectPlayerDataframe
            {
                startPosition = CheckPosition
            };
            _gameClient.Send(ref dataframe);
        }
        else
        {
            _isPlayerDetected = false;

            if (MovementType.waypointsSequentalPatrolling == movementType)
                _enemyMovement.SequentalWaypointsMovement();
            else if (MovementType.clockwise == movementType)
                _enemyMovement.ClockwiseWaypointsMovement();
        }
    }

    private void Update()
    {
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
            soundService.PlaySound(mouseType == PlayerMouseType.ThinMouse ? SoundType.ThinDetect : SoundType.FatDetect);
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
        _enemyFOV.DrawFOV(_enemyScan.ViewDistance, _enemyScan.ViewAngle, _enemyScan.ObstacleLayer);
        var cameraService = GameContainer.InGame.Resolve<GameCamera>();
        if (cameraService != null && cameraService.Camera != null) _markController.LookAt(cameraService.Camera);
    }

    private void OnPlayerDetected(ref PlayerDetectedMessage value)
    {
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
        if (!_canTriggerAlert)
            return;

        if (_gameClient.IsConnected)
        {
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