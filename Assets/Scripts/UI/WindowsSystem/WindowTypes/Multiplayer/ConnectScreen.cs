using Common;
using LocalMessages;
using NetFrame.Client;
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
    public class ConnectScreen : WindowBase
    {
        [SerializeField] private TMP_InputField _nicknameText;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _cancelButton;

        private void Awake()
        {
            _connectButton.onClick.AddListener(Connect);
            _cancelButton.onClick.AddListener(Cancel);

            var messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            messageBroker.Subscribe<ConnectedMessage>(OnConnected);
            messageBroker.Subscribe<ConnectionFailedMessage>(OnConnectionFailed);
        }

        private void Connect()
        {
            var gameClient = GameContainer.Common.Resolve<GameClient>();
            gameClient.Connect();
        }

        private void Cancel()
        {
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.DestroyWindow(this);
        }

        private void OnConnected(ref ConnectedMessage message)
        {
            var dataframe = new PlayerInfoDataframe
            {
                name = _nicknameText.text
            };
            GameContainer.Common.Resolve<NetFrameClient>().Send(ref dataframe);
            
            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<RoomsListScreen>();
            windowsSystem.DestroyWindow(this);
        }

        private void OnConnectionFailed(ref ConnectionFailedMessage message)
        {
            var notificationsManager = GameContainer.Common.Resolve<NotificationsManager>();
            notificationsManager.ShowNotification($"Connection failed: {message.reason}", NotificationsManager.NotificationType.Center);
        }
    }
}