using GameCore.Character.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour, IAnimationSource
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
    float animationSpeed; 
    public AnimationType CurrentAnimation => AnimationType.Walk;
    [SerializeField] float walkAnimationSpeed;
    [SerializeField] CharacterVisuals visuals;

    public float AnimationSpeed => animationSpeed;
    #endregion

    public void Init(float moveSpeed)
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        visuals.Initialize(this);
    }

    public void SequentalWaypointsMovement()
    {
        if (canMove == false) return;

        if (movePoints.Count == 0)
            return;

        if (Vector3.Distance(transform.position, movePoints[currentPointIndex].transform.position) > 2f)
        {
            animationSpeed = walkAnimationSpeed;
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

    public void ClockwiseWaypointsMovement()
    {
        if (canMove == false) return;

        if (movePoints.Count == 0)
            return;

        if (Vector3.Distance(transform.position, movePoints[currentPointIndex].transform.position) > 2f)
        {
            animationSpeed = 1f;
            agent.SetDestination(movePoints[currentPointIndex].transform.position);
        }
        else
        {
            if (movePoints[currentPointIndex].NeedStay)
            {
                var coroutine = StartCoroutine(WaitOnPoint(movePoints[currentPointIndex].StayTime));
                currentCoroutine = coroutine;
            }

            if (currentPointIndex == movePoints.Count - 1)
            {
                currentPointIndex = 0;
            }
            else
            {
                currentPointIndex++;
            }
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        animationSpeed = 1f;
        agent.SetDestination(target);
        if(Vector3.Distance(transform.position, target) < 1.5f)
        {
            animationSpeed = 0f;
        }
    }

    private IEnumerator WaitOnPoint(float time)
    {
        canMove = false;
        animationSpeed = 0f;
        yield return new WaitForSeconds(time);
        animationSpeed = walkAnimationSpeed;
        canMove = true;
    }
}