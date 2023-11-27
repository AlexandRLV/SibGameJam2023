using Common.DI;
using LocalMessages;
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
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private LocalMessageBroker _messageBroker;

        private void Start()
        {
            _connectButton.onClick.AddListener(Connect);
            _cancelButton.onClick.AddListener(Cancel);

            _messageBroker.Subscribe<PlayerInfoRequestDataframe>(SendPlayerInfo);
            _messageBroker.Subscribe<PlayerInfoReceivedDataframe>(ProcessPlayerInfoReceived);
            _messageBroker.Subscribe<DisconnectByReasonDataframe>(DisconnectByReason);

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
            Debug.Log("Destroy connect window");
            _messageBroker.Unsubscribe<PlayerInfoRequestDataframe>(SendPlayerInfo);
            _messageBroker.Unsubscribe<PlayerInfoReceivedDataframe>(ProcessPlayerInfoReceived);
            _messageBroker.Unsubscribe<DisconnectByReasonDataframe>(DisconnectByReason);

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
            _notificationsManager.ShowNotification("Подключаемся к серверу...", NotificationsManager.NotificationType.Center, 0.5f);
        }

        private void Cancel()
        {
            _windowsSystem.DestroyWindow(this);
            _windowsSystem.CreateWindow<MainMenu>();
        }

        private void OnConnected(ref ConnectedMessage message)
        {
            Debug.Log("Connected!");
            _gameClientData.PlayerName = _nicknameText.text;
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

        private void SendPlayerInfo(ref PlayerInfoRequestDataframe obj)
        {
            Debug.Log("Sending player info");
            var dataframe = new PlayerInfoDataframe
            {
                name = _nicknameText.text,
                clientVersion = GameClient.ClientVersion,
            };
            _gameClient.Send(ref dataframe);
        }

        private void ProcessPlayerInfoReceived(ref PlayerInfoReceivedDataframe dataframe)
        {
            _windowsSystem.DestroyWindow(this);
            _windowsSystem.CreateWindow<RoomsListWindow>();
        }

        private void DisconnectByReason(ref DisconnectByReasonDataframe dataframe)
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