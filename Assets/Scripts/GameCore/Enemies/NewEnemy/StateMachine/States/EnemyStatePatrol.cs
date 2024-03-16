using GameCore.Character.Animation;
using GameCore.Enemies.EnemyObject;
using UnityEngine;

namespace GameCore.Enemies.NewEnemy.StateMachine.States
{
    public class EnemyStatePatrol : EnemyStateBase
    {
        public override AnimationType AnimationType => AnimationType.Walk;
        public override EnemyStateType Type => EnemyStateType.Patrol;

        private bool _reachedDestination;
        private bool _goingReverse;
        
        public EnemyStatePatrol(NewEnemyMovement enemyMovement) : base(enemyMovement)
        {
        }

        public override bool CanEnter(EnemyStateType prevState)
        {
            if (!movement.EnableWaypointsMovement) return false;
            if (movement.Waypoints == null || movement.Waypoints.Length == 0) return false;
            
            return preset.movementType is EnemyMovementType.WaypointsClockwise or EnemyMovementType.WaypointsSequential;
        }

        public override bool CanExit(EnemyStateType nextState)
        {
            if (nextState == EnemyStateType.Alarm) return true;
            return _reachedDestination && movement.CurrentWaypoint.NeedStay;
        }

        public override void OnEnter(EnemyStateType prevState)
        {
            movement.Agent.isStopped = false;
            
            if (movement.CurrentWaypointId < 0 || movement.CurrentWaypointId >= movement.Waypoints.Length)
            {
                movement.CurrentWaypointId = 0;
                movement.CurrentWaypoint = movement.Waypoints[0];
            }
            
            UpdateWaypoints();

            movement.Agent.SetDestination(movement.CurrentWaypoint.transform.position);
            _reachedDestination = false;
        }

        public override void Update()
        {
            float distance = Vector3.Distance(movement.transform.position, movement.CurrentWaypoint.transform.position);
            _reachedDestination = distance <= preset.stoppingDistance;
            
            if (!_reachedDestination) return;
            if (movement.CurrentWaypoint.NeedStay) return;
            
            UpdateWaypoints();
        }

        private void UpdateWaypoints()
        {
            float distance = Vector3.Distance(movement.transform.position, movement.CurrentWaypoint.transform.position);
            if (distance > preset.stoppingDistance) return;
            
            if (_goingReverse)
                movement.CurrentWaypointId--;
            else
                movement.CurrentWaypointId++;

            if (movement.CurrentWaypointId >= 0 && movement.CurrentWaypointId < movement.Waypoints.Length)
            {
                movement.CurrentWaypoint = movement.Waypoints[movement.CurrentWaypointId];
                movement.Agent.SetDestination(movement.CurrentWaypoint.transform.position);
                return;
            }
            
            if (preset.movementType == EnemyMovementType.WaypointsClockwise)
            {
                movement.CurrentWaypointId = movement.CurrentWaypointId < 0 ? movement.Waypoints.Length - 1 : 0;
            }
            else
            {
                _goingReverse = !_goingReverse;
                movement.CurrentWaypointId = movement.CurrentWaypointId < 0 ? 0 : movement.Waypoints.Length - 1;
            }

            movement.CurrentWaypoint = movement.Waypoints[movement.CurrentWaypointId];
            movement.Agent.SetDestination(movement.CurrentWaypoint.transform.position);
        }
    }
}