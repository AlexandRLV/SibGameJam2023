using System.Reflection;
using Common.DI;
using LocalMessages;
using NetFrame.Utils;
using Networking.Client;

#if UNITY_WEBGL
using Networking.Client.WebSocket;
#else
using Networking.Client.NetFrame;
#endif

using Networking.Rooms;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class ClientInitializer : InitializerBase
    {
        [SerializeField] private ClientParameters _clientParameters;
        
        private IGameClient _gameClient;
        private RoomController _roomController;
        private GameClientData _gameClientData;
        
        public override void Initialize()
        {
            NetFrameDataframeCollection.Initialize(Assembly.GetExecutingAssembly());
    
            var messageBroker = new LocalMessageBroker();
            GameContainer.Common.Register(messageBroker);

            GameContainer.Common.Register(_clientParameters);

            _gameClientData = new GameClientData();
            GameContainer.Common.Register(_gameClientData);
            
#if UNITY_WEBGL
            var monoUpdater = new GameObject("MonoUpdater").AddComponent<MonoUpdater>();
            GameContainer.Common.Register(monoUpdater);
            DontDestroyOnLoad(monoUpdater.gameObject);
            
            var nativeSocketClient = GameContainer.Create<NativeWebSocketClient>();
            _gameClient = nativeSocketClient;
#else
            var gameClient = GameContainer.CreateGameObjectWithComponent<GameClient>("GameClient");
            DontDestroyOnLoad(gameClient.gameObject);
            _gameClient = gameClient;
#endif
            
            GameContainer.Common.Register(_gameClient);
            
            _roomController = GameContainer.Create<RoomController>();
            GameContainer.Common.Register(_roomController);
        }

        public override void Dispose()
        {
            if (_gameClient != null && _gameClientData.IsConnected)
                _gameClient.Disconnect();
            
            _roomController?.Dispose();
        }
    }
}