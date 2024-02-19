using Common.DI;
using GameCore.Camera;
using GameCore.Character.Movement;
using Networking;
using Networking.Client;
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

        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private GameCamera _gameCamera;

        private int _tick;

        public void Initialize(CharacterMovement movement, PlayerMouseType mouseType)
        {
            MouseType = mouseType;
            
            CurrentMovement = movement;
            CurrentMovement.Posess();
            
            _gameCamera.SetTarget(CurrentMovement.transform);
        }

        public void Unposess()
        {
            if (CurrentMovement != null)
                CurrentMovement.Unposess();

            CurrentMovement = null;
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
            _gameClient.Send(ref dataframe);
        }

        private void SendCurrentPosition()
        {
            if (!_gameClientData.IsConnected)
                return;
            
            if (CurrentMovement == null)
                return;
            
            var dataframe = new PlayerPositionDataframe
            {
                Tick = _tick,
                Teleported = Teleported,
                Position = CurrentMovement.transform.position,
                Rotation = CurrentMovement.transform.rotation,
                animationType = (byte)CurrentMovement.CurrentAnimation,
                animationSpeed = CurrentMovement.AnimationSpeed
            };
            _gameClient.Send(ref dataframe);
            Teleported = false;
        }
    }
}