using GameCore.Character.Animation;
using System.Collections;
using System.Collections.Generic;
using Common.DI;
using GameCore.LevelObjects;
using UnityEngine;
using UnityEngine.AI;

public class PrisonMouseMovement : MonoBehaviour, IAnimationSource
{
    public AnimationType CurrentAnimation { get; set; }
    public float AnimationSpeed { get; private set; }

    [SerializeField] private CharacterVisuals _visuals;
    [SerializeField] private NavMeshAgent _agent;

    [Inject] private LevelObjectService _levelObjectService;

    private bool _evacuated;
    
    public void Init()
    {
        GameContainer.InjectToInstance(this);
        _visuals.Initialize(this);
    }

    public void Evacuate()
    {
        _evacuated = true;
        AnimationSpeed = 1f;
        _agent.SetDestination(_levelObjectService.evacuation.transform.position);
    }

    private void Update()
    {
        if (!_evacuated)
            return;

        if (Vector3.Distance(transform.position, _agent.destination) < 1f)
            AnimationSpeed = 0f;
    }
}