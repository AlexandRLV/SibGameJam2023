using System.Collections.Generic;
using Common;
using GameCore.Camera;
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
        public bool IsControlledByPlayer { get; private set; }
        public InputState InputState { get; private set; }
        public CharacterMoveValues MoveValues { get; private set; }
        public Rigidbody Rigidbody => _rigidbody;
        public CharacterParameters Parameters => _parameters;
        
        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CharacterParameters _parameters;

        [Header("Floating")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _floatingHeight;
        [SerializeField] private bool _applySpring;
        [SerializeField] private float _appliedYForce;
        
        [Header("State machine")]
        [SerializeField] private bool _debugStateChanges;
        [SerializeField] private MovementStateType _currentState;

        private StateMachine<MovementStateBase, MovementStateType> _stateMachine;

        private GameCamera _gameCamera;
        private Vector3 _movement;

#region Internal methods
        private void Awake()
        {
            MoveValues = new CharacterMoveValues
            {
                SpeedMultiplier = 1f,
                JumpHeightMultiplier = 1f,
                FloatingHeightMultiplier = 1f,
            };
            
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
            Unposess();
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            RotateToCamera();

            float gravity = Physics.gravity.y * _parameters.gravityMultiplier * _rigidbody.mass;
            _rigidbody.AddForce(Vector3.up * gravity);
            
            _stateMachine.CheckStates(_debugStateChanges);
            _stateMachine.FixedUpdate();
        }

        private void CheckGrounded()
        {
            _appliedYForce = 0f;
            float checkHeight = _floatingHeight * 1.5f * MoveValues.FloatingHeightMultiplier;
            var groundCheckOrigin = transform.position + Vector3.up * checkHeight;
            Physics.Raycast(groundCheckOrigin, Vector3.down, out var hit, _floatingHeight * 3f, _groundMask);

            if (hit.colliderInstanceID == 0)
            {
                Debug.DrawLine(groundCheckOrigin, groundCheckOrigin + Vector3.down * (_floatingHeight * 3f), Color.red);
                IsGrounded = false;
                return;
            }

            float distanceToGround = hit.distance;
            if (distanceToGround > checkHeight)
            {
                Debug.DrawLine(groundCheckOrigin, hit.point, Color.blue);
                IsGrounded = false;
                return;
            }

            Debug.DrawLine(groundCheckOrigin, hit.point, Color.green);
            IsGrounded = true;

            if (!_applySpring) return;

            float rayDirVelocity = Vector3.Dot(Vector3.down, _rigidbody.velocity);
            float yDelta = hit.distance - checkHeight;
            float springForce = yDelta * _parameters.springForce - rayDirVelocity * _parameters.dampingForce;
            _appliedYForce = springForce;

            springForce *= _rigidbody.mass;
            _rigidbody.AddForce(Vector3.down * springForce);
        }

        private void RotateToCamera()
        {
            if (!IsControlledByPlayer) return;
            if (_movement.magnitude < 0.1f) return;

            float targetY = _gameCamera.FollowTarget.transform.eulerAngles.y;
            _rigidbody.rotation = Quaternion.Euler(0f, targetY, 0f);
        }
#endregion

#region Public methods
        public void Posess()
        {
            InputState = GameContainer.InGame.Resolve<InputState>();
            IsControlledByPlayer = true;
            _rigidbody.drag = 0f;

            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
            _gameCamera.FollowTarget.Height = _parameters.cameraHeight;
        }

        public void Unposess()
        {
            InputState = null;
            IsControlledByPlayer = false;
            
            _rigidbody.velocity = Vector3.zero.WithY(_rigidbody.velocity.y);
            _rigidbody.drag = 5f;
        }
        
        public void Move(Vector2 input)
        {
            var rotation = _gameCamera.FollowTarget.transform.FlatRotation();
            _movement = rotation * new Vector3(input.x, 0f, input.y);
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, _movement, _parameters.lerpInertiaSpeed * Time.deltaTime);
        }

        public void MoveInAir(Vector2 input)
        {
            var rotation = _gameCamera.FollowTarget.transform.FlatRotation();
            _movement = rotation * new Vector3(input.x, 0f, input.y);
            var horizontalVelocity = _rigidbody.velocity;
            float verticalVelocity = horizontalVelocity.y;
            horizontalVelocity.y = 0f;

            horizontalVelocity = Vector3.Lerp(horizontalVelocity, _movement, _parameters.lerpInertiaSpeed * Time.deltaTime);
            _rigidbody.velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
        }
#endregion
    }
}