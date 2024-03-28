using System.Collections.Generic;
using Common;
using GameCore.Character.Visuals;
using GameCore.Enemies.NewEnemy.Parameters;
using GameCore.Enemies.NewEnemy.StateMachine;
using GameCore.Enemies.NewEnemy.StateMachine.States;
using GameCore.Enemies.RouteControl;
using GameCore.Player;
using UnityEngine;
using UnityEngine.AI;

namespace GameCore.Enemies.NewEnemy
{
    public class NewEnemyMovement : MonoBehaviour, IAnimationSource
    {
        public AnimationType CurrentAnimation { get; private set; }
        public float AnimationSpeed { get; private set; }

        public NewEnemyVision Vision => _vision;
        public NavMeshAgent Agent => _agent;
        public EnemyMainPreset Preset { get; private set; }
        public IPlayer Player { get; private set; }
        
        public bool EnableWaypointsMovement { get; private set; }
        public Waypoint[] Waypoints { get; private set; }
        
        public int CurrentWaypointId { get; set; }
        public Waypoint CurrentWaypoint { get; set; }
        
        [SerializeField] private NewEnemyVision _vision;
        [SerializeField] private NavMeshAgent _agent;

        private bool _initialized;
        private EnemyStateMachine _enemyStateMachine;
        
        public void Initialize(EnemyMainPreset preset, CharacterVisuals visuals, IPlayer player)
        {
            Preset = preset;
            
            visuals.Initialize(this);
            visuals.transform.SetParent(transform);
            visuals.transform.ToLocalZero();
            
            if (visuals.KnockdownEffect != null)
                visuals.KnockdownEffect.SetActive(false);

            _agent.speed = preset.walkSpeed;
            
            Player = player;
            _vision.Initialize(player, preset.viewPreset, visuals);

            _enemyStateMachine = new EnemyStateMachine
            {
                states = new List<EnemyStateBase>
                {
                    new EnemyStateIdle(this),
                    new EnemyStatePatrol(this),
                    new EnemyStateAlarm(this),
                }
            };
            _enemyStateMachine.ForceSetState(EnemyStateType.Idle);
            _initialized = true;
        }

        public void SetWaypointsMovement(Waypoint[] waypoints)
        {
            Waypoints = waypoints;
            EnableWaypointsMovement = true;
            CurrentWaypoint = waypoints[0];
            CurrentWaypointId = 0;
        }

        private void Update()
        {
            if (!_initialized) return;
            
            _enemyStateMachine.Update();
            _enemyStateMachine.CheckStates();

            CurrentAnimation = _enemyStateMachine.currentState.AnimationType;
            AnimationSpeed = _agent.velocity.magnitude / _agent.speed;
        }
    }
}