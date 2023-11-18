using GameCore.Character.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrisonMouseMovement : MonoBehaviour, IAnimationSource
{
    [SerializeField] private float pointMovementRange;
    [SerializeField] private Transform prisonFloor;
    [SerializeField] private CharacterVisuals visuals;

    private NavMeshAgent _agent;
    private Vector3 _movePoint;
    private Vector3 _evacuationPoint;
    private float _animationSpeed;
    private bool _findNext = true;

    public AnimationType CurrentAnimation => AnimationType.Walk;

    public float AnimationSpeed => _animationSpeed;

    public void Init()
    {
        _agent = GetComponent<NavMeshAgent>();
        visuals.Initialize(this);
        FindRandomPointInPrison();
    }

    public void PrisonMovement()
    {
        if (Vector3.Distance(_movePoint, _agent.transform.position) > pointMovementRange)
        {
            _animationSpeed = 1f;
            _agent.SetDestination(_movePoint);
        }
        else if (_findNext)
        {
            _findNext = false;
            Invoke(nameof(FindRandomPointInPrison), 2f);
            _animationSpeed = 0f;
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
        _movePoint = randomPoint;
    }

    private Vector3 RandomPointInBounds(Bounds bounds)
    {
        _findNext = true;
        return new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                           bounds.max.y,
                           Random.Range(bounds.min.z, bounds.max.z));

    }
}