using Common.DI;
using GameCore.Character.Visuals;
using GameCore.LevelObjects;
using GameCore.LevelObjects.FloorTypeDetection;
using UnityEngine;
using UnityEngine.AI;

namespace GameCore.Prison.Mouse
{
    public class PrisonMouseMovement : MonoBehaviour, IAnimationSource
    {
        public AnimationType CurrentAnimation { get; set; }
        public float AnimationSpeed { get; private set; }

        [SerializeField] private CharacterVisuals _visuals;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private FloorTypeDetector _floorTypeDetector;

        [Inject] private LevelObjectService _levelObjectService;

        private bool _evacuated;
    
        public void Init()
        {
            GameContainer.InjectToInstance(this);
            _visuals.Initialize(this, _floorTypeDetector);
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

            if (Vector3.Distance(transform.position, _agent.destination) < 3f)
                AnimationSpeed = 0f;
        }
    }
}