using Common.DI;
using GameCore.LevelObjects.Abstract;
using GameCore.LevelObjects.FloorTypeDetection;
using GameCore.Sounds.ChairMovementSound;
using Networking.Client;
using Networking.Client.NetFrame;
using Networking.Dataframes.InGame;
using UnityEngine;

namespace GameCore.LevelObjects.TriggerObjects
{
    public class PushableObject : BaseTriggerObject, ICheckPositionObject
    {
        private const int SendUpdateRate = 5;
        
        public Vector3 CheckPosition { get; private set; }
        
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private AudioSource _source;
        [SerializeField] private FloorTypeDetector _floorTypeDetector;
        [SerializeField] private ChairMovementSoundsData _soundsData;

        private bool _isOnline;
        private int _tick;

        private Vector3 _lastPosition;

        private IGameClient _client;
        private GameClientData _gameClientData;

        private void Start()
        {
            _soundsData.Initialize();
        }

        protected override void OnPlayerEnter()
        {
            if (!Movement.Parameters.canPush)
                return;
            
            _rigidbody.isKinematic = false;
            _tick = 0;
        }

        private void FixedUpdate()
        {
            if (_rigidbody.isKinematic) return;
            
            UpdateSound();
            
            if (!_isOnline) return;
            
            _tick++;
            if (_tick % SendUpdateRate != 0) return;

            var dataframe = new PushablePositionDataframe
            {
                startPosition = CheckPosition,
                position = _rigidbody.position,
                Rotation = _rigidbody.rotation,
            };
            _client.Send(ref dataframe);
        }

        private void UpdateSound()
        {
            var position = transform.position;
            if (position == _lastPosition || _soundsData == null)
            {
                _source.Stop();
                return;
            }

            _lastPosition = position;
            var floorType = _floorTypeDetector.GetCurrentType();
            var clip = _soundsData.GetClipForFloorType(floorType);

            if (_source.clip != clip)
            {
                _source.Stop();
                _source.clip = clip;
            }
            
            if (!_source.isPlaying)
                _source.Play();
        }

        protected override void OnPlayerExit()
        {
            if (!_rigidbody.isKinematic && _isOnline)
            {
                var dataframe = new PushablePositionDataframe
                {
                    startPosition = CheckPosition,
                    position = _rigidbody.position,
                    Rotation = _rigidbody.rotation,
                };
                _client.Send(ref dataframe);
            }
            _rigidbody.isKinematic = true;
        }

        private void OnEnable()
        {
            GameContainer.InGame.Resolve<LevelObjectService>().RegisterPushableObject(this);

            _client = GameContainer.Common.Resolve<GameClient>();
            _gameClientData = GameContainer.Common.Resolve<GameClientData>();
            _isOnline = _gameClientData.IsConnected;
            
            CheckPosition = transform.position;
        }

        private void OnDisable()
        {
            if (GameContainer.InGame.CanResolve<LevelObjectService>())
                GameContainer.InGame.Resolve<LevelObjectService>().UnregisterPushableObject(this);
        }
    }
}