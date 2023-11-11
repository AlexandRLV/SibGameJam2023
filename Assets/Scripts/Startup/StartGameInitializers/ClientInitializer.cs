using System.Collections;
using Common;
using Networking;
using LocalMessages;
using NetFrame.Client;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class ClientInitializer : IInitializer
    {
        private GameClient _gameClient;
        
        public IEnumerator Initialize()
        {
            var messageBrocker = new LocalMessageBroker();
            GameContainer.Common.Register(messageBrocker);

            var netClient = new NetFrameClient();
            GameContainer.Common.Register(netClient);

            var parameters = Resources.Load<ClientParameters>("Client Parameters");
            var gameClientGO = new GameObject();
            _gameClient = gameClientGO.AddComponent<GameClient>();
            _gameClient.Initialize(parameters);
            
            Object.DontDestroyOnLoad(gameClientGO);
            
            GameContainer.Common.Register(_gameClient);
            
            yield return null;
        }

        public void Dispose()
        {
            if (_gameClient != null)
                _gameClient.Shutdown();
        }
    }
}