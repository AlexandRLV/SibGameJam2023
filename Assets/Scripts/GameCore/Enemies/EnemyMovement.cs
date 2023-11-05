using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    #region Public Variables

    [HideInInspector] public List<Waypoint> movePoints;

    #endregion

    #region Private Variables

    int currentPointIndex;
    bool clockwiseMovement = true;
    bool canMove = true;
    NavMeshAgent agent;
    [SerializeField] Coroutine currentCoroutine;

    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SequentalWaypointsMovement()
    {
        if (canMove == false) return;

        if (movePoints.Count == 0)
            return;

        if (Vector3.Distance(transform.position, movePoints[currentPointIndex].transform.position) > 2f)
        {
            agent.SetDestination(movePoints[currentPointIndex].transform.position);
        }
        else
        {
            if (movePoints[currentPointIndex].NeedStay)
            {
                var coroutine = StartCoroutine(WaitOnPoint(movePoints[currentPointIndex].StayTime));
                currentCoroutine = coroutine;
            }

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

    /*
    public IEnumerator NoWalkRotation(float minAngle, float maxAngle, float rotationSpeed)
    {
        float randomAngle = Random.Range(minAngle, maxAngle);
        Vector3 newVector = new Vector3(0, randomAngle, 0f);

        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(newVector), 
            rotationSpeed * Time.deltaTime);
    }
    */

    public void MoveToTarget(Vector3 target)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        agent.SetDestination(target);
    }

    private IEnumerator WaitOnPoint(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}