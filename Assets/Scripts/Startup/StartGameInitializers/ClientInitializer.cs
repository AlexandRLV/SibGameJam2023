using System.Reflection;
using Common.DI;
using Networking;
using LocalMessages;
using NetFrame.Utils;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class ClientInitializer : InitializerBase
    {
        private IGameClient _gameClient;
        private RoomController _roomController;
        
        public override void Initialize()
        {
            NetFrameDataframeCollection.Initialize(Assembly.GetExecutingAssembly());
    
            var messageBroker = new LocalMessageBroker();
            GameContainer.Common.Register(messageBroker);

            var parameters = Resources.Load<ClientParameters>("Client Parameters");
            GameContainer.Common.Register(parameters);

            var clientData = new GameClientData();
            GameContainer.Common.Register(clientData);
            
#if UNITY_WEBGL
            var monoUpdater = new GameObject("MonoUpdater").AddComponent<MonoUpdater>();
            GameContainer.Common.Register(monoUpdater);
            Object.DontDestroyOnLoad(monoUpdater.gameObject);
            
            var nativeSocketClient = GameContainer.Create<NativeWebSocketClient>();
            _gameClient = nativeSocketClient;
#else
            var gameClientPrefab = Resources.Load<GameClient>("Prefabs/GameClient");
            var gameClient = GameContainer.InstantiateAndResolve(gameClientPrefab);
            Object.DontDestroyOnLoad(gameClient.gameObject);
            _gameClient = gameClient;
#endif
            
            GameContainer.Common.Register(_gameClient);
            
            _roomController = GameContainer.Create<RoomController>();
            GameContainer.Common.Register(_roomController);
        }

        public override void Dispose()
        {
            _gameClient?.Disconnect();
            _roomController?.Dispose();
        }
    }
}