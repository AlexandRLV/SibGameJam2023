using Common;
using Common.DI;
using GameCore.Character.Movement;
using GameCore.Character.Visuals;
using GameCore.LevelObjects.FloorTypeDetection;
using LocalMessages;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Player.Network
{
    public class RemotePlayer : MonoBehaviour, IAnimationSource
    {
        public AnimationType CurrentAnimation { get; private set; }
        public float AnimationSpeed { get; private set; }

        [SerializeField] private CharacterPositionInterpolator _interpolator;
        [SerializeField] private FloorTypeDetector _floorTypeDetector;
        
        [Inject] private LocalMessageBroker _messageBroker;
        
        private CharacterVisuals _visuals;
        
        public void Initialize(CharacterVisuals visuals)
        {
            _visuals = visuals;
            _visuals.transform.SetParent(transform);
            _visuals.transform.ToLocalZero();
            _visuals.Initialize(this, _floorTypeDetector);
            
            _messageBroker.Subscribe<PlayerPositionDataframe>(ProcessPlayerPosition);
            _messageBroker.Subscribe<SetCurrentTickDataframe>(SetOwnerTick);
            _messageBroker.Subscribe<PlayerEffectStateDataframe>(SetEffectState);
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<PlayerPositionDataframe>(ProcessPlayerPosition);
            _messageBroker.Unsubscribe<SetCurrentTickDataframe>(SetOwnerTick);
            _messageBroker.Unsubscribe<PlayerEffectStateDataframe>(SetEffectState);
        }

        private void Update()
        {
            ref var snapshot = ref _interpolator.Current;
            transform.SetPositionAndRotation(snapshot.Position, snapshot.Rotation);
            CurrentAnimation = snapshot.AnimationType;
            AnimationSpeed = snapshot.animationSpeed;
        }

        private void ProcessPlayerPosition(ref PlayerPositionDataframe dataframe)
        {
            _interpolator.AddSnapshot(dataframe);
        }

        private void SetOwnerTick(ref SetCurrentTickDataframe dataframe)
        {
            _interpolator.SetOwnerTick(dataframe.tick);
        }

        private void SetEffectState(ref PlayerEffectStateDataframe dataframe)
        {
            if (dataframe.Type == EffectType.Knockdown)
            {
                _visuals.KnockdownEffect.SetActive(dataframe.active);
                if (dataframe.active) _visuals.KnockdownEffect.GetComponent<ParticleSystem>().Play();
            }
            else if (dataframe.Type == EffectType.SpeedUp)
            {
                if (_visuals.SpeedUp != null)
                    _visuals.SpeedUp.SetActive(dataframe.active);
            }
        }
    }
}