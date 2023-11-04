using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    #region Serialize Variables

    [Header("Waypoints Patrolling")]
    [SerializeField] List<Transform> movePoints;

    [Header("Random Patrolling")]
    [SerializeField] float range;
    [SerializeField] Transform centrePoint;

    #endregion

    #region Private Variables

    int currentPointIndex;
    bool clockwiseMovement = true;
    NavMeshAgent agent;

    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SequentalWaypointsMovement()
    {
        if (movePoints.Count == 0)
            return;

        if (Vector3.Distance(transform.position, movePoints[currentPointIndex].position) > 1f)
        {
            agent.SetDestination(movePoints[currentPointIndex].position);
        }
        else
        {
            if (clockwiseMovement == true)
            {
                if (currentPointIndex == movePoints.Count - 1)
                {
                    clockwiseMovement = false;
                    return;
                }
                currentPointIndex++;
            }
            else if (clockwiseMovement == false)
            {
                if (currentPointIndex == 0)
                {
                    clockwiseMovement = true;
                    return;
                }
                currentPointIndex--;
            }
        }
    }
}