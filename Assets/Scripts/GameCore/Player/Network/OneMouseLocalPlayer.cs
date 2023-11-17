using Common;
using GameCore.Camera;
using GameCore.Character.Movement;
using NetFrame.Client;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Player.Network
{
    public class OneMouseLocalPlayer : MonoBehaviour
    {
        [HideInInspector] public bool Teleported;
        [SerializeField] private NetworkParameters _parameters;

        private CharacterMovement _movement;
        private NetFrameClient _client;
        private GameCamera _gameCamera;

        private int _tick;

        public void Initialize(CharacterMovement movement)
        {
            _movement = movement;
            _movement.Posess();
            
            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
            _gameCamera.SetTarget(_movement.transform);
        }
        
        private void Awake()
        {
            _client = GameContainer.Common.Resolve<NetFrameClient>();
        }

        private void FixedUpdate()
        {
            _tick++;
            if (_tick % _parameters.SendTickUpdateRate == 0)
                SendCurrentTick();
            
            if (_tick % _parameters.SendPositionUpdateRate == 0)
                SendCurrentPosition();
        }

        private void SendCurrentTick()
        {
            var dataframe = new SetCurrentTickDataframe
            {
                tick = _tick
            };
            _client.Send(ref dataframe);
        }

        private void SendCurrentPosition()
        {
            var dataframe = new PlayerPositionDataframe
            {
                Tick = _tick,
                Teleported = Teleported,
                Position = _movement.transform.position,
                Rotation = _movement.transform.rotation,
                animationType = _movement.CurrentAnimation,
                animationSpeed = _movement.AnimationSpeed
            };
            _client.Send(ref dataframe);
            Teleported = false;
        }
    }
}