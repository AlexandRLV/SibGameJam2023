using Common;
using GameCore.Character.Movement;
using LocalMessages;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum MovementType
{
    waypointsSequentalPatrolling,
    noWalk
}

public class EnemyController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] MovementType movementType = MovementType.waypointsSequentalPatrolling;
    [SerializeField] float timeToAlert;
    [SerializeField] Color normalConeColor, alertConeColor;

    [SerializeField] float remainingTimeToAlert;
    [SerializeField] bool isPlayerDeteted = false;
    [SerializeField] bool isAlert = false;


    [Header("Patrolling Type")]

    [Header("NoWalk Type")]
    [Tooltip("Как часто объект поворачивает голову в разные стороны, в сек")]
    [SerializeField] float rotationRate;
    [SerializeField] float rotationSpeed;
    [SerializeField] float minAngle, maxAngle;

    float timer;
    bool canTurn;

    EnemyTargetScaner enemyScan;
    EnemyMovement enemyMovement;
    EnemyFOV enemyFOV;
    [SerializeField]Transform currentTarget;
    LocalMessageBroker _messageBroker;
    MarkController markController;

    public void Init(List<Waypoint> movePoints)
    {
        enemyScan = GetComponent<EnemyTargetScaner>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyFOV = GetComponentInChildren<EnemyFOV>();
        enemyMovement.movePoints = movePoints;
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
            isPlayerDeteted = true;
        }
        else
        {
            isPlayerDeteted = false;

            if (MovementType.waypointsSequentalPatrolling == movementType)
            {
                enemyMovement.SequentalWaypointsMovement();
            }
            else if (MovementType.noWalk == movementType)
            {

            }
        }
    }

    private void Update()
    {
        if (isPlayerDeteted && !isAlert)
        {
            CountRemainingTimeToAlert();
        }
        else
        {
            remainingTimeToAlert = timeToAlert;
        }
    }

    private void LateUpdate()
    {
        enemyFOV.DrawFOV(enemyScan.ViewDistance, enemyScan.ViewAngle, enemyScan.ObstacleLayer);
    }

    private void OnPlayerDetected(ref PlayerDetectedMessage value)
    {
        isAlert = true;
        enemyMovement.MoveToTarget(value.PlayerPosition);
        enemyFOV.SetColor(alertConeColor);
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

    private void OnDestroy()
    {
        _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
    }

    public void StartAlert()
    {
        var message = new PlayerDetectedMessage();
        message.PlayerPosition = currentTarget.position;
        _messageBroker.Trigger(ref message);
    }
}
