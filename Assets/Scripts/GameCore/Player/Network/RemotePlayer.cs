using Common;
using GameCore.Character.Animation;
using NetFrame.Client;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Player.Network
{
    public class RemotePlayer : MonoBehaviour, IAnimationSource
    {
        public AnimationType CurrentAnimation { get; private set; }
        public float AnimationSpeed { get; private set; }

        [SerializeField] private CharacterPositionInterpolator _interpolator;
        
        private NetFrameClient _client;
        private CharacterVisuals _visuals;
        
        public void Initialize(CharacterVisuals visuals)
        {
            _visuals = visuals;
            _visuals.transform.SetParent(transform);
            _visuals.transform.ToLocalZero();
            _visuals.Initialize(this);
            
            _client = GameContainer.Common.Resolve<NetFrameClient>();
            _client.Subscribe<PlayerPositionDataframe>(ProcessPlayerPosition);
            _client.Subscribe<SetCurrentTickDataframe>(SetOwnerTick);
        }

        private void OnDestroy()
        {
            _client.Unsubscribe<PlayerPositionDataframe>(ProcessPlayerPosition);
            _client.Unsubscribe<SetCurrentTickDataframe>(SetOwnerTick);
        }

        private void Update()
        {
            ref var snapshot = ref _interpolator.Current;
            transform.SetPositionAndRotation(snapshot.Position, snapshot.Rotation);
            CurrentAnimation = snapshot.animationType;
            AnimationSpeed = snapshot.animationSpeed;
        }

        private void ProcessPlayerPosition(PlayerPositionDataframe dataframe)
        {
            _interpolator.AddSnapshot(dataframe);
        }

        private void SetOwnerTick(SetCurrentTickDataframe dataframe)
        {
            _interpolator.SetOwnerTick(dataframe.tick);
        }
    }
}