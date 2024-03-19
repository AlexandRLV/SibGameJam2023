using System.Collections;
using System.Collections.Generic;
using Common;
using Common.DI;
using GameCore.Camera;
using GameCore.Character.Animation;
using GameCore.Character.Movement.States;
using GameCore.Common.Messages;
using GameCore.Input;
using GameCore.LevelAchievements.LocalMessages;
using GameCore.LevelObjects.Abstract;
using GameCore.Sounds;
using GameCore.StateMachine;
using LocalMessages;
using Networking.Client;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Character.Movement
{
    public class CharacterMovement : MonoBehaviour, IAnimationSource
    {
        public bool IsControlledByPlayer { get; private set; }
        public InputState InputState => _inputState;
        public CharacterMoveValues MoveValues { get; private set; }
        public CharacterLives Lives { get; private set; }
        public GameClientData GameClientData => _gameClientData;
        public IGameClient GameClient => _gameClient;
        
        public Rigidbody Rigidbody => _rigidbody;
        public CharacterParameters Parameters => _parameters;
        public Collider Collider => _collider;
        public StepSounds StepSounds => _visuals.StepSounds;
        public CharacterPhysicsBody PhysicsBody => _physicsBody;

        public AnimationType CurrentAnimation => _stateMachine.CurrentState.AnimationType;
        public float AnimationSpeed => IsControlledByPlayer ? InputState.moveVector.magnitude : 0f;
        
        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private CharacterParameters _parameters;
        [SerializeField] private CharacterGroundChecker _groundChecker;
        [SerializeField] private CharacterPhysicsBody _physicsBody;
        
        [Inject] private GameCamera _gameCamera;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private InputState _inputState;

        private StateMachine<MovementStateBase, MovementStateType> _stateMachine;

        private bool _isSpeedModified;
        
        private CharacterVisuals _visuals;
        private Vector3 _movement;

        public void Initialize(CharacterVisuals visuals)
        {
            _visuals = visuals;
            _visuals.transform.SetParent(transform);
            _visuals.transform.ToLocalZero();
            _visuals.Initialize(this);
            
            _groundChecker.Initialize(this);

            MoveValues = new CharacterMoveValues
            {
                speedMultiplier = 1f,
                lerpInertiaSpeed = _parameters.lerpInertiaSpeed,
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
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.N))
                Damage();
            
            _stateMachine.CheckStates();
        }

        private void FixedUpdate()
        {
            _stateMachine.Update();
        }

        public void Damage()
        {
            Lives.Lives--;
            Lives.LivesChanged?.Invoke();
            
            if (!IsControlledByPlayer) return;

            var damagedMessage = new PlayerDamagedMessage();
            _messageBroker.Trigger(ref damagedMessage);
            
            if (Lives.Lives > 0) return;

            MoveValues.isKnockdown = true;
            var message = new PlayerDeadMessage();
            _messageBroker.Trigger(ref message);
        }

        public void Posess()
        {
            IsControlledByPlayer = true;
            _rigidbody.drag = 0f;
    
            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
            _gameCamera.FollowTarget.Height = _parameters.cameraHeight;
            
            if (MoveValues.currentInteractiveObject != null)
                MoveValues.currentInteractiveObject.SetInteractIndicatorState(true);
        }

        public void Unposess()
        {
            IsControlledByPlayer = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.drag = 100f;
            
            if (MoveValues.currentInteractiveObject != null)
                MoveValues.currentInteractiveObject.SetInteractIndicatorState(false);
        }

        public void ChangeMovementSpeed(float multiplier, float duration)
        {
            if (_isSpeedModified) return;
            
            if (multiplier > 1f)
            {
                SetEffectState(EffectType.SpeedUp, true);
                
                if (GameClientData.IsConnected)
                {
                    var dataframe = new PlayerEffectStateDataframe
                    {
                        type = (byte)EffectType.SpeedUp,
                        active = true,
                    };
                    GameClient.Send(ref dataframe);
                }
            }
            
            MoveValues.speedMultiplier = multiplier;
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

        public void SetCurrentInteractiveObject(InteractiveObject value)
        {
            if (MoveValues.currentInteractiveObject != null)
                MoveValues.currentInteractiveObject.SetInteractIndicatorState(false);
            
            MoveValues.currentInteractiveObject = value;
            
            if (value != null)
                value.SetInteractIndicatorState(true);
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
            MoveValues.speedMultiplier = 1f;
            _isSpeedModified = false;

            if (!GameClientData.IsConnected)
                yield break;
            
            var dataframe = new PlayerEffectStateDataframe
            {
                type = (byte)EffectType.SpeedUp,
                active = false,
            };
            GameClient.Send(ref dataframe);
        }
    }
}