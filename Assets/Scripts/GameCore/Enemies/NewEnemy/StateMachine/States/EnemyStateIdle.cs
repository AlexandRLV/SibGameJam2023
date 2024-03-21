using GameCore.Character.Visuals;
using GameCore.Enemies.EnemyObject;
using UnityEngine;

namespace GameCore.Enemies.NewEnemy.StateMachine.States
{
    public class EnemyStateIdle : EnemyStateBase
    {
        public override AnimationType AnimationType => AnimationType.Walk;
        public override EnemyStateType Type => EnemyStateType.Idle;

        private float _waitOnWaypointTimer;
        private float _headRotationTimer;
        private float _headRotationDelay;
        private bool _invertRotation;
        
        public EnemyStateIdle(NewEnemyMovement enemyMovement) : base(enemyMovement)
        {
        }

        public override bool CanExit(EnemyStateType nextState)
        {
            if (nextState == EnemyStateType.Alarm) return true;
            if (movement.Preset.movementType == EnemyMovementType.NoWalk) return false;

            return _waitOnWaypointTimer <= 0;
        }

        public override void OnEnter(EnemyStateType prevState)
        {
            movement.Agent.isStopped = true;
            _headRotationTimer = 0f;
            _headRotationDelay = 0f;
            
            if (movement.Preset.movementType == EnemyMovementType.NoWalk) return;
            if (movement.CurrentWaypoint != null && movement.CurrentWaypoint.NeedStay)
                _waitOnWaypointTimer = movement.CurrentWaypoint.StayTime;
        }

        public override void OnExit(EnemyStateType nextState)
        {
            movement.Vision.HeadRotationAngle = 0f;
        }

        public override void Update()
        {
            if (_waitOnWaypointTimer > 0f)
                _waitOnWaypointTimer -= Time.deltaTime;
            
            if (!movement.Preset.hasHeadRotationPreset) return;

            if (_headRotationDelay > 0f)
            {
                _headRotationDelay -= Time.deltaTime;
                if (_headRotationDelay > 0f) return;
            }

            var headPreset = preset.headRotationPreset;

            _headRotationTimer += _invertRotation ? -Time.deltaTime : Time.deltaTime;
            float t = _headRotationTimer / headPreset.headRotationTime;

            if (t >= 1)
            {
                _invertRotation = true;
                _headRotationDelay = headPreset.headRotationInterval;
            }
            else if (t <= -1)
            {
                _invertRotation = false;
                _headRotationDelay = headPreset.headRotationInterval;
            }

            bool invertAngle = t < 0;
            if (invertAngle) t *= -1;
            
            float angle = headPreset.headRotationAngle * headPreset.headRotationCurve.Evaluate(t);
            movement.Vision.HeadRotationAngle = invertAngle ? -angle : angle;
        }
    }
}