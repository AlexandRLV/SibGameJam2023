using GameCore.Character.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrisonMouseMovement : MonoBehaviour, IAnimationSource
{
    [SerializeField] float pointMovementRange;
    [SerializeField] Transform prisonFloor;
    [SerializeField] CharacterVisuals visuals;

    NavMeshAgent agent;
    Vector3 movePoint;
    Vector3 evacuationPoint;
    float animationSpeed;
    bool findNext = true;

    public AnimationType CurrentAnimation => AnimationType.Walk;

    public float AnimationSpeed => animationSpeed;

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        visuals.Initialize(this);
        FindRandomPointInPrison();
    }

    public void PrisonMovement()
    {
        if (Vector3.Distance(movePoint, agent.transform.position) > pointMovementRange)
        {
            animationSpeed = 1f;
            agent.SetDestination(movePoint);
        }
        else if (findNext == true)
        {
            findNext = false;
            Invoke(nameof(FindRandomPointInPrison), 2f);
            animationSpeed = 0f;
        }
    }

    public void EvacuationMovement()
    {
        
    }

    private void FindRandomPointInPrison()
    {
        if (prisonFloor == null) return;
        Collider collider = prisonFloor.GetComponent<Collider>();
        Vector3 randomPoint = RandomPointInBounds(collider.bounds);
        movePoint = randomPoint;
    }

    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        findNext = true;
        return new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                           bounds.max.y,
                           Random.Range(bounds.min.z, bounds.max.z));

    }
}
