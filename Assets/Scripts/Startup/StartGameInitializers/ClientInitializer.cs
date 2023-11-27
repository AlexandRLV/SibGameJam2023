﻿using System.Collections;
using System.Reflection;
using Common;
using Common.DI;
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
        private RoomController _roomController;
        
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

            // Загрузка сетевых параметров
            var parameters = Resources.Load<ClientParameters>("Client Parameters");
            GameContainer.Common.Register(parameters);

            // Создание внутриигрового клиента
            var gameClientPrefab = Resources.Load<GameClient>("Prefabs/GameClient");
            _gameClient = GameContainer.InstantiateAndResolve(gameClientPrefab);
            Object.DontDestroyOnLoad(_gameClient);
            GameContainer.Common.Register(_gameClient);

            var webSocketClient = GameContainer.Create<WebSocketGameClient>();
            webSocketClient.Connect();
            GameContainer.Common.Register(webSocketClient);

            // Создание контроллера комнат
            _roomController = GameContainer.Create<RoomController>();
            GameContainer.Common.Register(_roomController);

            yield return null;
        }

        public void Dispose()
        {
            if (_gameClient != null)
                _gameClient.Shutdown();

            _roomController?.Dispose();
        }
    }
}