using System.Collections.Generic;
using Common;
using Common.DI;
using Localization;
using LocalMessages;
using NetFrame.Enums;
using Networking.Client;
using Networking.Client.NetFrame;
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

        private List<LocalizationParameter> _parameters;

        private void Start()
        {
            _parameters = new List<LocalizationParameter>
            {
                new LocalizationParameter
                {
                    key = "reason",
                    value = ""
                }
            };
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
                _notificationsManager.ShowNotification(Const.Notifications.LobbyEnterNicknameError); // "$LOBBY_ROOM_ENTER_NICKNAME_ERROR"
                return;
            }
            
            _gameClient.Connect();
            _notificationsManager.ShowNotification(Const.Notifications.LobbyConnectingToServer); //"$LOBBY_ROOM_CONNECTING_TO_SERVER"
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
                ReasonServerConnectionFailed.AlreadyConnected => "$LOBBY_CONNECTION_ALREADY_CONNECTED",
                ReasonServerConnectionFailed.ConnectionLost => "$LOBBY_CONNECTION_TO_SERVER_LOST",
                _ => "$LOBBY_CONNECTION_FAILED"
            };
            
            Debug.Log($"Connection failed by reason: {reason}");

            _parameters[0].value = reason;
            _notificationsManager.ShowNotification(Const.Notifications.ConnectToServerError, _parameters); // "$LOBBY_CONNECTION_TO_SERVER_ERROR"
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
                DisconnectReason.ClientVersion => "$LOBBY_CONNECTION_OLD_CLIENT_VERSION",
                DisconnectReason.NicknameTaken => "$LOBBY_DISCONNECTED_NICKNAME_TAKEN",
                _ => "$LOBBY_DISCONNECTED_SERVER_ERROR"
            };
            
            _parameters[0].value = reason;
            _notificationsManager.ShowNotification(Const.Notifications.ConnectToServerError, _parameters); // "$LOBBY_CONNECTION_TO_SERVER_ERROR"
        }
    }
}