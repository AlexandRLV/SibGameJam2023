using Common;
using Common.DI;
using GameCore.LevelObjects;
using NetFrame.Client;
using Networking;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class PushableObject : BaseTriggerObject, ICheckPositionObject
    {
        private const int SendUpdateRate = 5;
        
        public Vector3 CheckPosition { get; private set; }
        
        [SerializeField] private Rigidbody _rigidbody;

        private bool _isOnline;
        private int _tick;

        private GameClient _client;
        
        protected override void OnPlayerEnter()
        {
            if (!Movement.Parameters.canPush)
                return;
            
            _rigidbody.isKinematic = false;
            _tick = 0;
        }

        private void FixedUpdate()
        {
            if (!_isOnline) return;
            if (_rigidbody.isKinematic) return;

            _tick++;
            if (_tick % SendUpdateRate != 0) return;

            var dataframe = new PushablePositionDataframe
            {
                startPosition = CheckPosition,
                position = _rigidbody.position,
                rotation = _rigidbody.rotation,
            };
            _client.Send(ref dataframe);
        }

        protected override void OnPlayerExit()
        {
            if (!_rigidbody.isKinematic && _isOnline)
            {
                var dataframe = new PushablePositionDataframe
                {
                    startPosition = CheckPosition,
                    position = _rigidbody.position,
                    rotation = _rigidbody.rotation,
                };
                _client.Send(ref dataframe);
            }
            _rigidbody.isKinematic = true;
        }

        private void OnEnable()
        {
            GameContainer.InGame.Resolve<LevelObjectService>().RegisterPushableObject(this);

            _client = GameContainer.Common.Resolve<GameClient>();
            _isOnline = _client.IsConnected;
            
            CheckPosition = transform.position;
        }

        private void OnDisable()
        {
            if (GameContainer.InGame.CanResolve<LevelObjectService>())
                GameContainer.InGame.Resolve<LevelObjectService>().UnregisterPushableObject(this);
        }
    }
}