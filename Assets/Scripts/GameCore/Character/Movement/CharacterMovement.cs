using System.Collections.Generic;
using Common;
using GameCore.Character.Movement.States;
using GameCore.Input;
using GameCore.StateMachine;
using UnityEngine;

namespace GameCore.Character.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }
        public bool IsDead { get; private set; }
        public InputState InputState { get; private set; }
        public Rigidbody Rigidbody => _rigidbody;
        public CharacterParameters Parameters => _parameters;
        
        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CharacterParameters _parameters;

        [Header("Floating")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _floatingHeight;
        [SerializeField] private float _appliedYForce;
        
        [Header("State machine")]
        [SerializeField] private bool _debugStateChanges;
        [SerializeField] private MovementStateType _currentState;

        private StateMachine<MovementStateBase, MovementStateType> _stateMachine;

        private void Awake()
        {
            InputState = GameContainer.InGame.Resolve<InputState>();

            _stateMachine = new StateMachine<MovementStateBase, MovementStateType>
            {
                States = new List<MovementStateBase>
                {
                    new MovementWalkState(this),
                    new MovementJumpState(this),
                    new MovementDeadState(this)
                }
            };
            
            _stateMachine.ForceSetState(MovementStateType.Walk, _debugStateChanges);
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            
            _stateMachine.CheckStates(_debugStateChanges);
            _stateMachine.FixedUpdate();
        }

        private void CheckGrounded()
        {
            float checkHeight = _floatingHeight * 1.5f;
            var groundCheckOrigin = transform.position + Vector3.up * checkHeight;
            Physics.Raycast(groundCheckOrigin, Vector3.down, out var hit, _floatingHeight * 3f, _groundMask);

            if (hit.colliderInstanceID == 0)
            {
                IsGrounded = false;
                return;
            }

            float distanceToGround = hit.distance;
            if (distanceToGround > checkHeight)
            {
                IsGrounded = false;
                return;
            }

            IsGrounded = true;

            float rayDirVelocity = Vector3.Dot(Vector3.down, _rigidbody.velocity);
            float yDelta = hit.distance - checkHeight;
            float springForce = yDelta * _parameters.springForce - rayDirVelocity * _parameters.dampingForce;
            _appliedYForce = springForce;
            
            _rigidbody.AddForce(Vector3.down * springForce);
        }

        public void Move(Vector2 input)
        {
            var movement = _rigidbody.rotation * new Vector3(input.x, 0f, input.y);
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, movement, _parameters.lerpInertiaSpeed * Time.deltaTime);
        }

        public void MoveInAir(Vector2 input)
        {
            var movement = _rigidbody.rotation * new Vector3(input.x, 0f, input.y);
            var horizontalVelocity = _rigidbody.velocity;
            float verticalVelocity = horizontalVelocity.y;
            horizontalVelocity.y = 0f;

            horizontalVelocity = Vector3.Lerp(horizontalVelocity, movement, _parameters.lerpInertiaSpeed * Time.deltaTime);
            _rigidbody.velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
        }
    }
}