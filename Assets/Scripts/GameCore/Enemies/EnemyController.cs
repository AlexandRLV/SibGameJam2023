using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    waypointsClockwisePatrolling
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] MovementType movementType = MovementType.waypointsClockwisePatrolling;

    EnemyTargetScaner enemyScan;
    EnemyMovement enemyMovement;

    private void Awake()
    {
        enemyScan = GetComponent<EnemyTargetScaner>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void FixedUpdate()
    {
        enemyScan.GetNearestTarget();

        if (MovementType.waypointsClockwisePatrolling == movementType)
        {
            enemyMovement.SequentalWaypointsMovement();
        }
    }
}
