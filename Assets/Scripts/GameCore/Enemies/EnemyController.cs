using Common;
using GameCore.Character.Movement;
using GameCore.Common;
using GameCore.Sounds;
using LocalMessages;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum MovementType
{
    waypointsSequentalPatrolling,
    clockwise,
    noWalk
}

public class EnemyController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] MovementType movementType = MovementType.waypointsSequentalPatrolling;
    [SerializeField] float timeToAlert;
    [SerializeField] float questionTimeAfterDetect;
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] Color normalConeColor, alertConeColor;

    float remainingTimeToAlert;
    float remainingTimeToShowQuestion;
    bool isPlayerDeteted = false;
    bool isAlert = false;

    /*
    [Header("Patrolling Type")]

    [Header("NoWalk Type")]
    [Tooltip("Как часто объект поворачивает голову в разные стороны, в сек")]
    [SerializeField] float rotationRate;
    [SerializeField] float rotationSpeed;
    [SerializeField] float minAngle, maxAngle;
    */
    float timer;
    bool canTurn;

    EnemyTargetScaner enemyScan;
    EnemyMovement enemyMovement;
    EnemyFOV enemyFOV;
    Transform currentTarget;
    LocalMessageBroker _messageBroker;
    MarkController markController;
    SoundService soundService => GameContainer.Common.Resolve<SoundService>();
    RoundController roundController => GameContainer.InGame.Resolve<RoundController>();

    private void Awake()
    {
        if (MovementType.noWalk == movementType) Init(null);
    }

    public void Init(List<Waypoint> movePoints)
    {
        enemyScan = GetComponent<EnemyTargetScaner>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyFOV = GetComponentInChildren<EnemyFOV>();
        markController = GetComponentInChildren<MarkController>();
        enemyMovement.movePoints = movePoints;
        enemyMovement.Init(moveSpeed);
        currentTarget = null;
        enemyFOV.Init(enemyScan.ViewAngle);
        enemyFOV.SetColor(normalConeColor);

        _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
        _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
    }

    private void FixedUpdate()
    {
        if (isAlert) return;
        currentTarget = enemyScan.GetNearestTarget();

        if (currentTarget != null)
        {
            if (!isPlayerDeteted)
            {
                if (roundController.Stage == RoundStage.ThinMouse)
                {
                    soundService.PlaySound(SoundType.ThinDetect);
                }
                else if (roundController.Stage == RoundStage.FatMouse)
                {
                    soundService.PlaySound(SoundType.FatDetect);
                }
            }
            isPlayerDeteted = true;
        }
        else
        {
            isPlayerDeteted = false;

            if (MovementType.waypointsSequentalPatrolling == movementType)
            {
                enemyMovement.SequentalWaypointsMovement();
            }
            else if (MovementType.clockwise == movementType)
            {
                enemyMovement.ClockwiseWaypointsMovement();
            }
            
        }
    }

    private void Update()
    {
        if (isAlert) return;

        if (isPlayerDeteted && isAlert == false)
        {
            markController.SetQuestionMark();
            CountRemainingTimeToAlert();
            remainingTimeToShowQuestion = questionTimeAfterDetect;
        }
        else
        {
            remainingTimeToAlert = timeToAlert;
            CountRemainingTimeToShowQuestion();
        }
    }

    private void LateUpdate()
    {
        enemyFOV.DrawFOV(enemyScan.ViewDistance, enemyScan.ViewAngle, enemyScan.ObstacleLayer);
        if (Camera.main != null) markController.LookAt(Camera.main.transform);
    }

    private void OnPlayerDetected(ref PlayerDetectedMessage value)
    {
        isAlert = true;
        enemyMovement.MoveToTarget(value.PlayerPosition);
        enemyFOV.SetColor(alertConeColor);
        markController.SetExclamationMark();
    }

    
    public void FoundPlayer(CharacterMovement movement)
    {
        var message = new PlayerDetectedMessage
        {
            PlayerPosition = movement.transform.position
        };
        _messageBroker.Trigger(ref message);
    }
    

    private void CountRemainingTimeToAlert()
    {
        remainingTimeToAlert -= Time.deltaTime;

        if (remainingTimeToAlert < 0)
        {
            StartAlert();

        }
    }

    private void CountRemainingTimeToShowQuestion()
    {
        if (remainingTimeToShowQuestion < 0)
        {
            markController.ResetMarks();
        }
        else remainingTimeToShowQuestion -= Time.deltaTime;
    }

    private void OnDestroy()
    {
        _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
    }

    public void StartAlert()
    {
        soundService.StopSound();
        soundService.PlaySound(SoundType.Alert);
        var message = new PlayerDetectedMessage();
        message.PlayerPosition = currentTarget.position;
        _messageBroker.Trigger(ref message);
    }
}
