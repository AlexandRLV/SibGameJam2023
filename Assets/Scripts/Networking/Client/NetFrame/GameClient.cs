using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.DI;
using GameCore.Common;
using LocalMessages;
using NetFrame;
using NetFrame.Client;
using NetFrame.Enums;
using Networking.Dataframes;
using Networking.Dataframes.InGame;
using Networking.LocalMessages;
using Startup;
using UI.NotificationsSystem;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UI.WindowsSystem.WindowTypes.Multiplayer.Rooms;
using UnityEngine;

namespace Networking
{
    public class GameClient : MonoBehaviour, IGameClient
    {
        public const string ClientVersion = "0.1.0";

        private NetFrameClient _client;
        
        private ClientParameters _parameters;
        private GameClientData _data;
        private LocalMessageBroker _messageBroker;
        
        private List<SubscriptionContainer> _subscribedDataframes;
        
        [Construct]
        public void Construct(ClientParameters parameters, GameClientData data, LocalMessageBroker messageBroker)
        {
            _parameters = parameters;
            _data = data;
            _messageBroker = messageBroker;
            _client = new NetFrameClient();
        }

        private void Update()
        {
            if (_data.IsConnected)
                _client?.Run();
        }

        public void Connect()
        {
            _client.ConnectionSuccessful += OnConnectionSuccessful;
            _client.ConnectedFailed += OnConnectionFailed;
            _client.Disconnected += OnDisconnected;
            
            _client.Subscribe<PlayerLeftRoomDataframe>(OnPlayerLeftRoom);
            _client.Subscribe<GameFinishedDataframe>(OnGameFinished);
            _client.Subscribe<LoseGameDataframe>(OnLoseGame);
            
            var dataframes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(INetworkDataframe).IsAssignableFrom(t) && t.IsValueType)
                .ToList();

            var subscribeMethod = _client.GetType().GetMethod(nameof(_client.Subscribe));
            var resendMethod = GetType().GetMethod(nameof(RedirectToMessageBroker),
                BindingFlags.NonPublic | BindingFlags.Instance);

            _subscribedDataframes = new List<SubscriptionContainer>();

            foreach (var type in dataframes)
            {
                var resendGenericMethod = resendMethod.MakeGenericMethod(type);
                var actionType = typeof(Action<,>).MakeGenericType(type, typeof(int));
                var handler = Delegate.CreateDelegate(actionType, this, resendGenericMethod);
                var subscribeGenericMethod = subscribeMethod.MakeGenericMethod(type);
                subscribeGenericMethod.Invoke(_client, new[] { handler });
            
                _subscribedDataframes.Add(new SubscriptionContainer
                {
                    dataframeType = type,
                    handler = handler,
                });
            }
            
            _client.Connect(_parameters.Ip, _parameters.port);
        }

        public void Disconnect()
        {
            _client.Disconnect();
            
            _client.ConnectionSuccessful -= OnConnectionSuccessful;
            _client.ConnectedFailed -= OnConnectionFailed;
            _client.Disconnected -= OnDisconnected;
            
            _client.Unsubscribe<PlayerLeftRoomDataframe>(OnPlayerLeftRoom);
            _client.Unsubscribe<GameFinishedDataframe>(OnGameFinished);
            _client.Unsubscribe<LoseGameDataframe>(OnLoseGame);
            
            var unsubscribeMethod = _client.GetType().GetMethod(nameof(_client.Unsubscribe));
            foreach (var container in _subscribedDataframes)
            {
                var unsubscribeGenericMethod = unsubscribeMethod.MakeGenericMethod(container.dataframeType);
                unsubscribeGenericMethod.Invoke(_client, new[] { container.handler });
            }
        }

        public void Send<T>(ref T dataframe) where T : struct, INetworkDataframe => _client.Send(ref dataframe);

        private void OnConnectionSuccessful()
        {
            Debug.Log("Connection successfull, sending connected message");
            _data.IsConnected = true;
            var message = new ConnectedMessage();
            _messageBroker.Trigger(ref message);
        }

        private void OnConnectionFailed(ReasonServerConnectionFailed reason)
        {
            if (reason == ReasonServerConnectionFailed.AlreadyConnected)
                return;
            
            Debug.Log("Connection failed, sending failed message");
            _data.IsConnected = false;
            var message = new ConnectionFailedMessage
            {
                reason = reason
            };
            _messageBroker.Trigger(ref message);
        }

        private void OnDisconnected()
        {
            Debug.Log("Disconnected, sending disconnect message");
            _data.IsConnected = false;
            var message = new DisconnectedMessage();
            _messageBroker.Trigger(ref message);
        }

        private void OnPlayerLeftRoom(PlayerLeftRoomDataframe dataframe)
        {
            Debug.Log("Finishing game, because partner left!");
            FinishGameByReason(GameFinishedReason.Leave);
        }

        private void OnGameFinished(GameFinishedDataframe dataframe)
        {
            Debug.Log("Game finished dataframe"); 
            FinishGameByReason(dataframe.reason);
        }

        private void FinishGameByReason(GameFinishedReason reason)
        {
            Debug.Log($"Game finished by reason: {reason}");
            string notif = reason switch
            {
                GameFinishedReason.Win => "Миссия пройдена!",
                GameFinishedReason.Lose => "Миссия провалена!",
                GameFinishedReason.Leave => "Напарник вышел из игры",
                GameFinishedReason.YouLeft => "Ты вышел из игры",
            };
            
            var notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
            notificationsManager.ShowNotification(notif, NotificationsManager.NotificationType.Center);
            
            var gameInitializer = GameContainer.Common.Resolve<GameInitializer>();
            if (!gameInitializer.InGame) return;
            
            gameInitializer.StopGame();
            
            Debug.Log("Creating room list window");
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow<MainMenu>();
            windowsSystem.CreateWindow<RoomsListWindow>();
        }

        private void OnLoseGame(LoseGameDataframe dataframe)
        {
            if (GameContainer.InGame == null) return;
            if (!GameContainer.InGame.CanResolve<RoundController>()) return;

            Debug.Log($"Game lose by reason: {dataframe.Reason}");
            var roundController = GameContainer.InGame.Resolve<RoundController>();
            roundController.LoseGameByReason(dataframe.Reason, false);
        }

        private void OnDestroy() => Disconnect();

        private void OnApplicationQuit() => Disconnect();
        
        private void RedirectToMessageBroker<T>(T dataframe) where T : struct, INetworkDataframe
        {
            _messageBroker.Trigger(ref dataframe);
        }
    }
}