using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrisonMouseMovement : MonoBehaviour
{
    [SerializeField] float pointMovementRange;
    [SerializeField] Transform prisonFloor;

    NavMeshAgent agent;
    Vector3 movePoint;
    Vector3 evacuationPoint;

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        FindRandomPointInPrison();
        evacuationPoint = EvacuationPointController.Instance.EvacuationPoint.position;
    }

    public void PrisonMovement()
    {
        if (Vector3.Distance(movePoint, agent.transform.position) > pointMovementRange)
        {
            agent.SetDestination(movePoint);
        }
        else
        {
            Invoke(nameof(FindRandomPointInPrison), 2f);
        }
    }

    public void EvacuationMovement()
    {
        if (Vector3.Distance(transform.position, evacuationPoint) > pointMovementRange)
        {
            agent.SetDestination(evacuationPoint);
        }
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
        return new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                           bounds.max.y,
                           Random.Range(bounds.min.z, bounds.max.z));
    }
}
