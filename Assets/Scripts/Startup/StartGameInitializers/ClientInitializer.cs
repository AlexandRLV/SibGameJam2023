﻿using System.Collections;
using System.Reflection;
using Common;
using Networking;
using LocalMessages;
using NetFrame.Client;
using NetFrame.Utils;
using UnityEngine;

namespace Startup.StartGameInitializers
{
    public class ClientInitializer : IInitializer
    {
        private GameClient _gameClient;
        
        public IEnumerator Initialize()
        {
            // Инициализация сетевых сообщений
            NetFrameDataframeCollection.Initialize(Assembly.GetExecutingAssembly());
    
            // Создание и регистрация локальной системы сообщений
            var messageBroker = new LocalMessageBroker();
            GameContainer.Common.Register(messageBroker);

            // Создание сетевого клиента
            var netClient = new NetFrameClient();
            GameContainer.Common.Register(netClient);

            // Создание внутриигрового клиента
            var parameters = Resources.Load<ClientParameters>("Client Parameters");
            GameContainer.Common.Register(parameters);
            
            var gameClientGO = new GameObject("GameClient");
            _gameClient = gameClientGO.AddComponent<GameClient>();
            _gameClient.Initialize();
            Object.DontDestroyOnLoad(gameClientGO);
            GameContainer.Common.Register(_gameClient);

            var roomController = new RoomController();
            roomController.Initialize();
            GameContainer.Common.Register(roomController);

            yield return null;
        }

        public void Dispose()
        {
            if (_gameClient != null)
                _gameClient.Shutdown();
        }
    }
}