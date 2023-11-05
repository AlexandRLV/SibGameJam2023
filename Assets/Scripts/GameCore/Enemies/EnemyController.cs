using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    waypointsSequentalPatrolling
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] MovementType movementType = MovementType.waypointsSequentalPatrolling;

    EnemyTargetScaner enemyScan;
    EnemyMovement enemyMovement;
    [SerializeField] Transform currentTarget;

    public void Init(List<Waypoint> movePoints)
    {
        enemyScan = GetComponent<EnemyTargetScaner>();
        enemyMovement = GetComponent<EnemyMovement>();
        enemyMovement.movePoints = movePoints;
        currentTarget = null;
    }

    private void FixedUpdate()
    {
        currentTarget = enemyScan.GetNearestTarget();

        if (MovementType.waypointsSequentalPatrolling == movementType)
        {
            if (currentTarget != null)
            {
                enemyMovement.MoveToTarget(currentTarget);
            }
            else enemyMovement.SequentalWaypointsMovement();

        }
    }
}
