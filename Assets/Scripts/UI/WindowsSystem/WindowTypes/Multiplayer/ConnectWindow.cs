using Common.DI;
using LocalMessages;
using NetFrame.Client;
using NetFrame.Enums;
using Networking;
using Networking.Dataframes;
using Networking.LocalMessages;
using TMPro;
using UI.NotificationsSystem;
using UI.WindowsSystem.WindowTypes.Multiplayer.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes.Multiplayer
{
    public class ConnectWindow : WindowBase
    {
        [SerializeField] private TMP_InputField _nicknameText;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _cancelButton;

        private NetFrameClient _client;
        private LocalMessageBroker _messageBroker;

        private void Start()
        {
            _connectButton.onClick.AddListener(Connect);
            _cancelButton.onClick.AddListener(Cancel);

            _client = GameContainer.Common.Resolve<NetFrameClient>();
            _client.Subscribe<PlayerInfoRequestDataframe>(SendPlayerInfo);
            _client.Subscribe<PlayerInfoReceivedDataframe>(ProcessPlayerInfoReceived);
            _client.Subscribe<DisconnectByReasonDataframe>(DisconnectByReason);

            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<ConnectedMessage>(OnConnected);
            _messageBroker.Subscribe<ConnectionFailedMessage>(OnConnectionFailed);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.KeypadEnter)) return;
            if (!_nicknameText.isFocused) return;
            
            Connect();
        }

        private void OnDestroy()
        {
            _client.Unsubscribe<PlayerInfoRequestDataframe>(SendPlayerInfo);
            _client.Unsubscribe<PlayerInfoReceivedDataframe>(ProcessPlayerInfoReceived);
            _client.Unsubscribe<DisconnectByReasonDataframe>(DisconnectByReason);

            _messageBroker.Unsubscribe<ConnectedMessage>(OnConnected);
            _messageBroker.Unsubscribe<ConnectionFailedMessage>(OnConnectionFailed);
        }

        private void Connect()
        {
            if (string.IsNullOrWhiteSpace(_nicknameText.text))
            {
                var notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
                notificationsManager.ShowNotification("Введите никнейм!", NotificationsManager.NotificationType.Center);
                return;
            }
            
            var gameClient = GameContainer.Common.Resolve<GameClient>();
            gameClient.Connect();
        }

        private void Cancel()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<MainMenu>();
            windowsSystem.DestroyWindow(this);
        }

        private void OnConnected(ref ConnectedMessage message)
        {
            var gameClient = GameContainer.Common.Resolve<GameClient>();
            gameClient.PlayerName = _nicknameText.text;
        }

        private void OnConnectionFailed(ref ConnectionFailedMessage message)
        {
            string reason = message.reason switch
            {
                ReasonServerConnectionFailed.AlreadyConnected => "Вы уже подключены",
                ReasonServerConnectionFailed.ConnectionLost => "Потеряно соединение с сервером",
                ReasonServerConnectionFailed.ImpossibleToConnect => "Невозможно подключиться к серверу"
            };
            
            Debug.Log($"Connection failed by reason: {reason}");
            
            var notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
            notificationsManager.ShowNotification($"Ошибка подключения: {reason}", NotificationsManager.NotificationType.Center);
        }

        private void SendPlayerInfo(PlayerInfoRequestDataframe obj)
        {
            var dataframe = new PlayerInfoDataframe
            {
                name = _nicknameText.text,
                clientVersion = GameClient.ClientVersion,
            };
            _client.Send(ref dataframe);
        }

        private void ProcessPlayerInfoReceived(PlayerInfoReceivedDataframe dataframe)
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<RoomsListWindow>();
            windowsSystem.DestroyWindow(this);
        }

        private void DisconnectByReason(DisconnectByReasonDataframe dataframe)
        {
            string reason = dataframe.reason switch
            {
                DisconnectReason.ClientVersion => "Версия клиента устарела, пожалуйста, обновите клиент",
                DisconnectReason.NicknameTaken => "Такой никнейм уже занят, выберите другой!"
            };
            
            var notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
            notificationsManager.ShowNotification($"Ошибка подключения: {reason}", NotificationsManager.NotificationType.Center, 3f);
        }
    }
}