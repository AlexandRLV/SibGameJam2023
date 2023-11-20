using Common;
using GameCore.Camera;
using GameCore.Character.Movement;
using NetFrame.Client;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.Player.Network
{
    public class OneMouseLocalPlayer : MonoBehaviour, IPlayer
    {
        public PlayerMouseType MouseType { get; private set; }
        public CharacterMovement CurrentMovement { get; private set; }

        [HideInInspector] public bool Teleported;
        [SerializeField] private NetworkParameters _parameters;

        private NetFrameClient _client;
        private GameCamera _gameCamera;

        private int _tick;

        public void Initialize(CharacterMovement movement, PlayerMouseType mouseType)
        {
            MouseType = mouseType;
            
            CurrentMovement = movement;
            CurrentMovement.Posess();
            
            _gameCamera = GameContainer.InGame.Resolve<GameCamera>();
            _gameCamera.SetTarget(CurrentMovement.transform);
        }

        public void Unposess()
        {
            if (CurrentMovement != null)
                CurrentMovement.Unposess();

            CurrentMovement = null;
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
                Position = CurrentMovement.transform.position,
                Rotation = CurrentMovement.transform.rotation,
                animationType = CurrentMovement.CurrentAnimation,
                animationSpeed = CurrentMovement.AnimationSpeed
            };
            _client.Send(ref dataframe);
            Teleported = false;
        }
    }
}