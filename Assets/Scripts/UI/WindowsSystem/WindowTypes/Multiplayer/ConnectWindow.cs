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

        [Inject] private NotificationsManager _notificationsManager;

        private void Start()
        {
            _connectButton.onClick.AddListener(Connect);
            _cancelButton.onClick.AddListener(Cancel);

            _gameClient.Client.Subscribe<PlayerInfoRequestDataframe>(SendPlayerInfo);
            _gameClient.Client.Subscribe<PlayerInfoReceivedDataframe>(ProcessPlayerInfoReceived);
            _gameClient.Client.Subscribe<DisconnectByReasonDataframe>(DisconnectByReason);

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
            _gameClient.Client.Unsubscribe<PlayerInfoRequestDataframe>(SendPlayerInfo);
            _gameClient.Client.Unsubscribe<PlayerInfoReceivedDataframe>(ProcessPlayerInfoReceived);
            _gameClient.Client.Unsubscribe<DisconnectByReasonDataframe>(DisconnectByReason);

            _messageBroker.Unsubscribe<ConnectedMessage>(OnConnected);
            _messageBroker.Unsubscribe<ConnectionFailedMessage>(OnConnectionFailed);
        }

        private void Connect()
        {
            if (string.IsNullOrWhiteSpace(_nicknameText.text))
            {
                _notificationsManager.ShowNotification("Введите никнейм!", NotificationsManager.NotificationType.Center);
                return;
            }
            
            _gameClient.Connect();
        }

        private void Cancel()
        {
            _windowsSystem.CreateWindow<MainMenu>();
            _windowsSystem.DestroyWindow(this);
        }

        private void OnConnected(ref ConnectedMessage message)
        {
            _gameClient.PlayerName = _nicknameText.text;
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
            
            _notificationsManager.ShowNotification($"Ошибка подключения: {reason}", NotificationsManager.NotificationType.Center);
        }

        private void SendPlayerInfo(PlayerInfoRequestDataframe obj)
        {
            var dataframe = new PlayerInfoDataframe
            {
                name = _nicknameText.text,
                clientVersion = GameClient.ClientVersion,
            };
            _gameClient.Client.Send(ref dataframe);
        }

        private void ProcessPlayerInfoReceived(PlayerInfoReceivedDataframe dataframe)
        {
            _windowsSystem.CreateWindow<RoomsListWindow>();
            _windowsSystem.DestroyWindow(this);
        }

        private void DisconnectByReason(DisconnectByReasonDataframe dataframe)
        {
            string reason = dataframe.reason switch
            {
                DisconnectReason.ClientVersion => "Версия клиента устарела, пожалуйста, обновите клиент",
                DisconnectReason.NicknameTaken => "Такой никнейм уже занят, выберите другой!"
            };
            
            _notificationsManager.ShowNotification($"Ошибка подключения: {reason}", NotificationsManager.NotificationType.Center, 3f);
        }
    }
}