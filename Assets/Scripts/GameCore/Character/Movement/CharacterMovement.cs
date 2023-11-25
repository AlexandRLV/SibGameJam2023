using System.Collections;
using System.Collections.Generic;
using Common;
using Common.DI;
using GameCore.Camera;
using GameCore.Character.Animation;
using GameCore.Character.Movement.States;
using GameCore.Common.Messages;
using GameCore.Input;
using GameCore.Sounds;
using GameCore.StateMachine;
using LocalMessages;
using Networking;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Character.Movement
{
    public class CharacterMovement : MonoBehaviour, IAnimationSource
    {
        public bool IsGrounded { get; private set; }
        public bool IsControlledByPlayer { get; private set; }
        public InputState InputState => _inputState;
        public CharacterMoveValues MoveValues { get; private set; }
        public CharacterLives Lives { get; private set; }
        public GameClient GameClient => _gameClient;
        
        public Rigidbody Rigidbody => _rigidbody;
        public CharacterParameters Parameters => _parameters;
        public Collider Collider => _collider;
        public StepSounds StepSounds => _visuals.StepSounds;

        public AnimationType CurrentAnimation => _stateMachine.CurrentState.AnimationType;
        public float AnimationSpeed => IsControlledByPlayer ? InputState.moveVector.magnitude : 0f;
        
        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private CharacterParameters _parameters;

        [Header("Floating")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _floatingHeight;
        [SerializeField] private bool _applySpring;
        
        [Inject] private GameCamera _gameCamera;
        [Inject] private GameClient _gameClient;
        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private InputState _inputState;

        private StateMachine<MovementStateBase, MovementStateType> _stateMachine;

        private bool _isSpeedModified;
        
        private CharacterVisuals _visuals;
        private Vector3 _movement;

#region Internal methods
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.N))
                Damage();
            
            _stateMachine.CheckStates();
        }

        private void FixedUpdate()
        {
            CheckGrounded();

            float gravity = Physics.gravity.y * _parameters.gravityMultiplier * _rigidbody.mass;
            _rigidbody.AddForce(Vector3.up * gravity);
            
            _stateMachine.Update();
        }

        private void CheckGrounded()
        {
            bool hitTriggers = Physics.queriesHitTriggers;
            Physics.queriesHitTriggers = false;
            float checkHeight = _floatingHeight * 1.5f * MoveValues.FloatingHeightMultiplier;
            var groundCheckOrigin = transform.position + Vector3.up * checkHeight;
            Physics.Raycast(groundCheckOrigin, Vector3.down, out var hit, _floatingHeight * 3f, _groundMask);
            Physics.queriesHitTriggers = hitTriggers;

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

            springForce *= _rigidbody.mass;
            _rigidbody.AddForce(Vector3.down * springForce);
        }

        private IEnumerator BuffTimer(float buffDuration)
        {
            float countdownValue = buffDuration;
            while (countdownValue > 0)
            {
                yield return null;
                countdownValue -= Time.deltaTime;
            }

            SetEffectState(EffectType.SpeedUp, false);
            MoveValues.SpeedMultiplier = 1f;
            _isSpeedModified = false;

            if (!GameClient.IsConnected)
                yield break;
            var dataframe = new PlayerEffectStateDataframe
            {
                type = EffectType.SpeedUp,
                active = false,
            };
            GameClient.Send(ref dataframe);
        }
#endregion

#region Public methods
        public void Initialize(CharacterVisuals visuals)
        {
            _visuals = visuals;
            _visuals.transform.SetParent(transform);
            _visuals.transform.ToLocalZero();
            _visuals.Initialize(this);
                    
            _visuals.KnockdownEffect.SetActive(false);

            MoveValues = new CharacterMoveValues
            {
                SpeedMultiplier = 1f,
                JumpHeightMultiplier = 1f,
                FloatingHeightMultiplier = 1f,
            };

            Lives = new CharacterLives
            {
                Lives = _parameters.lives
            };
                    
            _stateMachine = new StateMachine<MovementStateBase, MovementStateType>
            {
                States = new List<MovementStateBase>
                {
                    new MovementIdleWaitState(this),
                    new MovementWalkState(this),
                    new MovementKnockdownState(this),
                    new MovementInteractState(this),
                    new MovementHitState(this),
                }
            };
                    
            if (_parameters.canJump)
                _stateMachine.States.Add(new MovementJumpState(this));
                    
            _stateMachine.ForceSetState(MovementStateType.Walk);

            _collider.height -= _floatingHeight;
            _collider.center += Vector3.up * (_floatingHeight * 0.5f);
        }

        public void Damage()
        {
            Lives.Lives--;
            Lives.LivesChanged?.Invoke();
            
            if (Lives.Lives > 0) return;

            MoveValues.IsKnockdown = true;
            var message = new PlayerDeadMessage();
            _messageBroker.Trigger(ref message);
        }
        
        public void Posess()
        {
            Debug.Log("Character posess");
            IsControlledByPlayer = true;
            _rigidbody.drag = 0f;
    
            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
            _gameCamera.FollowTarget.Height = _parameters.cameraHeight;
        }

        public void Unposess()
        {
            Debug.Log("Character unposess");
            IsControlledByPlayer = false;
            _rigidbody.velocity = Vector3.zero;
        }

        public void Move(Vector2 input)
        {
            if (!IsControlledByPlayer)
                return;
            
            var rotation = _gameCamera.FollowTarget.transform.FlatRotation();
            _movement = rotation * new Vector3(input.x, 0f, input.y);
            var horizontalVelocity = _rigidbody.velocity;
            float verticalVelocity = horizontalVelocity.y;
            horizontalVelocity.y = 0f;

            horizontalVelocity = Vector3.Lerp(horizontalVelocity, _movement, _parameters.lerpInertiaSpeed * Time.deltaTime);
            _rigidbody.velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
            
            if (horizontalVelocity.magnitude < 0.1f) return;
            _rigidbody.rotation = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
        }

        public void ChangeMovementSpeed(float multiplier, float duration)
        {
            if (_isSpeedModified) return;
            
            if (multiplier > 1f)
            {
                SetEffectState(EffectType.SpeedUp, true);
                
                if (GameClient.IsConnected)
                {
                    var dataframe = new PlayerEffectStateDataframe
                    {
                        type = EffectType.SpeedUp,
                        active = true,
                    };
                    GameClient.Send(ref dataframe);
                }
            }
            
            MoveValues.SpeedMultiplier = multiplier;
            _isSpeedModified = true;
            StartCoroutine(BuffTimer(duration));
        }

        public void SetEffectState(EffectType effect, bool state)
        {
            if (effect == EffectType.Knockdown)
            {
                _visuals.KnockdownEffect.SetActive(state);
                if (state) _visuals.KnockdownEffect.GetComponent<ParticleSystem>().Play();
            }
            else if (effect == EffectType.SpeedUp)
            {
                _visuals.SpeedUp.SetActive(state);
            }
        }
#endregion
    }
}