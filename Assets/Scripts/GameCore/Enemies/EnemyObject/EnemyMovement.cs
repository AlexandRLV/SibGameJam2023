using System.Collections;
using System.Collections.Generic;
using GameCore.Character.Visuals;
using GameCore.Enemies.RouteControl;
using UnityEngine;
using UnityEngine.AI;

namespace GameCore.Enemies.EnemyObject
{
    public class EnemyMovement : MonoBehaviour, IAnimationSource
    {
        public AnimationType CurrentAnimation => AnimationType.Walk;
        public float AnimationSpeed { get; private set; }

        [HideInInspector] public List<Waypoint> movePoints;
        
        [SerializeField] private float walkAnimationSpeed;
        [SerializeField] private CharacterVisuals visuals;

        private int _currentPointIndex;
        private bool _clockwiseMovement = true;
        private bool _canMove = true;
        
        private NavMeshAgent _agent;
        private Coroutine _currentCoroutine;

        public void Init(float moveSpeed)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = moveSpeed;
            visuals.Initialize(this, null);
        }

        public void SequentialWaypointsMovement()
        {
            if (!_canMove) return;
            if (movePoints.Count == 0) return;

            if (Vector3.Distance(transform.position, movePoints[_currentPointIndex].transform.position) > 2f)
            {
                AnimationSpeed = walkAnimationSpeed;
                _agent.SetDestination(movePoints[_currentPointIndex].transform.position);
            }
            else
            {
                if (movePoints[_currentPointIndex].NeedStay)
                    _currentCoroutine = StartCoroutine(WaitOnPoint(movePoints[_currentPointIndex].StayTime));

                if (_clockwiseMovement)
                {
                    if (_currentPointIndex == movePoints.Count - 1)
                    {
                        _clockwiseMovement = false;
                        return;
                    }
                    
                    _currentPointIndex++;
                }
                else
                {
                    if (_currentPointIndex == 0)
                    {
                        _clockwiseMovement = true;
                        return;
                    }
                    
                    _currentPointIndex--;
                }
            }
        }

        public void ClockwiseWaypointsMovement()
        {
            if (!_canMove) return;
            if (movePoints.Count == 0) return;

            if (Vector3.Distance(transform.position, movePoints[_currentPointIndex].transform.position) > 2f)
            {
                AnimationSpeed = 1f;
                _agent.SetDestination(movePoints[_currentPointIndex].transform.position);
            }
            else
            {
                if (movePoints[_currentPointIndex].NeedStay)
                    _currentCoroutine = StartCoroutine(WaitOnPoint(movePoints[_currentPointIndex].StayTime));

                if (_currentPointIndex == movePoints.Count - 1)
                    _currentPointIndex = 0;
                else
                    _currentPointIndex++;
            }
        }

        public void MoveToTarget(Vector3 target)
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
            
            AnimationSpeed = 1f;
            _agent.SetDestination(target);
            if(Vector3.Distance(transform.position, target) < 1.5f)
            {
                AnimationSpeed = 0f;
            }
        }

        private IEnumerator WaitOnPoint(float time)
        {
            _canMove = false;
            AnimationSpeed = 0f;
            yield return new WaitForSeconds(time);
            AnimationSpeed = walkAnimationSpeed;
            _canMove = true;
        }
    }
}