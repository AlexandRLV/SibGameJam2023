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
    public class GameClient : MonoBehaviour
    {
        public const string ClientVersion = "0.1.0";
        
        public string PlayerName { get; set; }
        public bool IsMaster { get; set; }
        public bool IsConnected { get; private set; }
        public NetFrameClient Client { get; private set; }
        
        private ClientParameters _parameters;
        
        [Construct]
        public void Construct(ClientParameters parameters, NetFrameClient client)
        {
            _parameters = parameters;
            Client = client;

            Client.ConnectionSuccessful += OnConnectionSuccessful;
            Client.ConnectedFailed += OnConnectionFailed;
            Client.Disconnected += OnDisconnected;
            
            Client.Subscribe<PlayerLeftRoomDataframe>(OnPlayerLeftRoom);
            Client.Subscribe<GameFinishedDataframe>(OnGameFinished);
            Client.Subscribe<LoseGameDataframe>(OnLoseGame);
        }

        private void Update()
        {
            Client?.Run();
        }

        public void Connect()
        {
            Client.Connect(_parameters.Ip, _parameters.port);
        }

        public void Disconnect()
        {
            Client.Disconnect();
        }

        public void Shutdown()
        {
            Client.ConnectionSuccessful -= OnConnectionSuccessful;
            Client.ConnectedFailed -= OnConnectionFailed;
            Client.Disconnected -= OnDisconnected;
            Client.Disconnect();
            
            Client.Unsubscribe<PlayerLeftRoomDataframe>(OnPlayerLeftRoom);
            Client.Unsubscribe<GameFinishedDataframe>(OnGameFinished);
            Client.Unsubscribe<LoseGameDataframe>(OnLoseGame);
        }

        public void Send<T>(ref T dataframe) where T : struct, INetworkDataframe => Client.Send(ref dataframe);

        private void OnConnectionSuccessful()
        {
            Debug.Log("Connection successfull, sending connected message");
            IsConnected = true;
            var message = new ConnectedMessage();
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }

        private void OnConnectionFailed(ReasonServerConnectionFailed reason)
        {
            if (reason == ReasonServerConnectionFailed.AlreadyConnected)
                return;
            
            IsConnected = false;
            Debug.Log("Connection failed, sending failed message");
            var message = new ConnectionFailedMessage
            {
                reason = reason
            };
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        }

        private void OnDisconnected()
        {
            Debug.Log("Disconnected, sending disconnect message");
            IsConnected = false;
            var message = new DisconnectedMessage();
            GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
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

            Debug.Log($"Game lose by reason: {dataframe.reason}");
            var roundController = GameContainer.InGame.Resolve<RoundController>();
            roundController.LoseGameByReason(dataframe.reason, false);
        }

        private void OnDestroy() => Shutdown();

        private void OnApplicationQuit() => Shutdown();
    }
}